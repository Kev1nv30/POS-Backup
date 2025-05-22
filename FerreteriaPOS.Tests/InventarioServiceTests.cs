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
    public class InventarioServiceTests
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
            
            await context.SaveChangesAsync();
        }
        
        [Fact]
        public async Task ReducirStock_ConStockSuficiente_DebeReducirStock()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestInventarioService(context);
            
            // Act
            var resultado = await service.ReducirStockAsync(1, 3);
            
            // Assert
            Assert.True(resultado);
            
            var producto = await context.Productos.FindAsync(1);
            Assert.Equal(7, producto?.Existencia);
            Assert.NotNull(producto?.UltimaModificacion);
        }
        
        [Fact]
        public async Task ReducirStock_ConStockInsuficiente_NoDebeReducirStock()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestInventarioService(context);
            
            // Act
            var resultado = await service.ReducirStockAsync(1, 15);
            
            // Assert
            Assert.False(resultado);
            
            var producto = await context.Productos.FindAsync(1);
            Assert.Equal(10, producto?.Existencia); // No debe cambiar
        }
        
        [Fact]
        public async Task VerificarStockSuficiente_ConStockSuficiente_DebeRetornarTrue()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestInventarioService(context);
            
            // Act
            var resultado = await service.VerificarStockSuficienteAsync(1, 5);
            
            // Assert
            Assert.True(resultado);
        }
        
        [Fact]
        public async Task VerificarStockSuficiente_ConStockInsuficiente_DebeRetornarFalse()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestInventarioService(context);
            
            // Act
            var resultado = await service.VerificarStockSuficienteAsync(2, 5);
            
            // Assert
            Assert.False(resultado);
        }
        
        [Fact]
        public async Task ActualizarStockProductos_ConStockSuficiente_DebeActualizarTodos()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestInventarioService(context);
            
            var items = new Dictionary<int, decimal>
            {
                { 1, 5 }
            };
            
            // Act
            var resultado = await service.ActualizarStockProductosAsync(items);
            
            // Assert
            Assert.True(resultado.Success);
            Assert.Empty(resultado.Errores);
            
            var producto = await context.Productos.FindAsync(1);
            Assert.Equal(5, producto?.Existencia);
        }
        
        [Fact]
        public async Task ActualizarStockProductos_ConStockInsuficiente_NoDebeActualizarNinguno()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = GetInMemoryContext(dbName);
            await SeedTestDataAsync(context);
            
            var service = new TestInventarioService(context);
            
            var items = new Dictionary<int, decimal>
            {
                { 1, 5 },
                { 2, 5 } // Producto 2 solo tiene 3 de existencia
            };
            
            // Act
            var resultado = await service.ActualizarStockProductosAsync(items);
            
            // Assert
            Assert.False(resultado.Success);
            Assert.Single(resultado.Errores);
            
            var producto1 = await context.Productos.FindAsync(1);
            var producto2 = await context.Productos.FindAsync(2);
            
            // Ninguno debe cambiar
            Assert.Equal(10, producto1?.Existencia);
            Assert.Equal(3, producto2?.Existencia);
        }
    }
} 