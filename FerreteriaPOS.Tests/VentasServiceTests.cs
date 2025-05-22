using FerreteriaPOS.Data;
using FerreteriaPOS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FerreteriaPOS.Tests
{
    public class VentasServiceTests
    {
        private FerreteriaContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<FerreteriaContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
                
            return new FerreteriaContext(options);
        }
        
        private async Task SeedTestDataAsync(FerreteriaContext context)
        {
            // Agregar productos de prueba
            context.Productos.Add(new Producto
            {
                Id = 1,
                CodigoBarra = "1111",
                Descripcion = "Producto 1",
                PrecioPublico = 100,
                PrecioMayoreo = 80,
                PrecioCosto = 50,
                Existencia = 10,
                MinStock = 5,
                MaxStock = 20,
                FechaCreacion = DateTime.Now
            });
            
            context.Productos.Add(new Producto
            {
                Id = 2,
                CodigoBarra = "2222",
                Descripcion = "Producto 2",
                PrecioPublico = 200,
                PrecioMayoreo = 160,
                PrecioCosto = 100,
                Existencia = 3,
                MinStock = 5,
                MaxStock = 20,
                FechaCreacion = DateTime.Now
            });
            
            // Agregar cliente de prueba
            context.Clientes.Add(new Cliente
            {
                Id = 1,
                Rut = "11.111.111-1",
                Nombre = "Cliente Prueba",
                Email = "cliente@test.com",
                Telefono = "123456789",
                Direccion = "Calle Prueba 123",
                Ventas = new List<Venta>()
            });
            
            await context.SaveChangesAsync();
        }
        
        [Fact]
        public async Task ProcesarVentaCompleta_DebeCrearVentaYReducirStock()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestVentasService(context);
            
            // Crear venta
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                ClienteId = 1,
                EsFactura = false,
                Usuario = "Test",
                Items = new List<VentaItem>()
            };
            
            // Crear items de venta
            var items = new List<VentaItem>
            {
                new VentaItem
                {
                    ProductoId = 1,
                    Cantidad = 2,
                    PrecioUnitario = 100,
                    CodigoBarra = "1111",
                    Descripcion = "Producto 1",
                    Subtotal = 200
                }
            };
            
            // Act
            var resultado = await service.ProcesarVentaCompletaAsync(venta, items);
            
            // Assert
            Assert.True(resultado.Success, $"Error: {resultado.Mensaje}");
            Assert.NotEqual(0, resultado.VentaId);
            
            // Verificar que se redujo el stock
            var producto = await context.Productos.FindAsync(1);
            Assert.Equal(8, producto?.Existencia);
            
            // Verificar que se guardó la venta
            var ventaGuardada = await context.Ventas
                .Include(v => v.Items)
                .FirstOrDefaultAsync(v => v.Id == resultado.VentaId);
                
            Assert.NotNull(ventaGuardada);
            Assert.NotNull(ventaGuardada?.Items);
            Assert.Single(ventaGuardada?.Items);
            Assert.Equal(200, ventaGuardada?.Total);
        }
        
        [Fact]
        public async Task ProcesarVentaCompleta_ConStockInsuficiente_NoDebeCrearVenta()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestVentasService(context);
            
            // Crear venta
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                ClienteId = 1,
                EsFactura = false,
                Usuario = "Test",
                Items = new List<VentaItem>()
            };
            
            // Crear items de venta con cantidad mayor al stock
            var items = new List<VentaItem>
            {
                new VentaItem
                {
                    ProductoId = 2, // Este producto solo tiene 3 de existencia
                    Cantidad = 5,
                    PrecioUnitario = 200,
                    CodigoBarra = "2222",
                    Descripcion = "Producto 2",
                    Subtotal = 1000
                }
            };
            
            // Act
            var resultado = await service.ProcesarVentaCompletaAsync(venta, items);
            
            // Assert
            Assert.False(resultado.Success);
            Assert.Equal(0, resultado.VentaId);
            
            // Verificar que no se redujo el stock
            var producto = await context.Productos.FindAsync(2);
            Assert.Equal(3, producto?.Existencia);
            
            // Verificar que no se guardó la venta
            var ventasCount = await context.Ventas.CountAsync();
            Assert.Equal(0, ventasCount);
        }
        
        [Fact]
        public async Task CancelarVenta_DebeEliminarVentaYRestaurarStock()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestVentasService(context);
            
            // Crear y procesar una venta primero
            var venta = new Venta
            {
                Fecha = DateTime.Now,
                ClienteId = 1,
                EsFactura = false,
                Usuario = "Test",
                Items = new List<VentaItem>()
            };
            
            var items = new List<VentaItem>
            {
                new VentaItem
                {
                    ProductoId = 1,
                    Cantidad = 2,
                    PrecioUnitario = 100,
                    CodigoBarra = "1111",
                    Descripcion = "Producto 1",
                    Subtotal = 200
                }
            };
            
            var resultadoVenta = await service.ProcesarVentaCompletaAsync(venta, items);
            
            Assert.True(resultadoVenta.Success, $"Error al crear venta: {resultadoVenta.Mensaje}");
            
            // Verificar el stock reducido
            var productoAntesCancel = await context.Productos.FindAsync(1);
            Assert.Equal(8, productoAntesCancel?.Existencia);
            
            // Act
            var resultadoCancelar = await service.CancelarVentaAsync(resultadoVenta.VentaId);
            
            // Assert
            Assert.True(resultadoCancelar.Success, $"Error al cancelar venta: {resultadoCancelar.Mensaje}");
            
            // Verificar que se restauró el stock
            var productoDespuesCancel = await context.Productos.FindAsync(1);
            Assert.Equal(10, productoDespuesCancel?.Existencia);
            
            // Verificar que se eliminó la venta
            var ventaEliminada = await context.Ventas.FindAsync(resultadoVenta.VentaId);
            Assert.Null(ventaEliminada);
        }
    }
} 