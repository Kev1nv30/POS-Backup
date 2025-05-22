using FerreteriaPOS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace FerreteriaPOS.Data
{
    public static class DbSeeder
    {
        public static void Seed(FerreteriaContext context)
        {
            // Asegurarse que la base de datos está creada
            context.Database.EnsureCreated();
            
            // Verificar y agregar productos de ejemplo si no hay ninguno
            if (!context.Productos.Any())
            {
                Console.WriteLine("Agregando productos de ejemplo...");
                
                // Agregar productos de ejemplo
                context.Productos.AddRange(
                    new Producto
                    {
                        CodigoBarra = "7501234567890",
                        Descripcion = "Martillo de carpintero 16oz",
                        PrecioCosto = 80.0m,
                        PrecioPublico = 150.0m,
                        PrecioMayoreo = 130.0m,
                        Existencia = 20,
                        MinStock = 5,
                        MaxStock = 50,
                        Departamento = "Herramientas",
                        Categoria = "Martillos",
                        TipoVenta = "Unidad",
                        Proveedor = "Truper",
                        FechaCreacion = DateTime.Now
                    },
                    new Producto
                    {
                        CodigoBarra = "7502234567891",
                        Descripcion = "Destornillador plano 6''",
                        PrecioCosto = 45.0m,
                        PrecioPublico = 85.0m,
                        PrecioMayoreo = 75.0m,
                        Existencia = 15,
                        MinStock = 10,
                        MaxStock = 30,
                        Departamento = "Herramientas",
                        Categoria = "Destornilladores",
                        TipoVenta = "Unidad",
                        Proveedor = "Pretul",
                        FechaCreacion = DateTime.Now
                    },
                    new Producto
                    {
                        CodigoBarra = "7503234567892",
                        Descripcion = "Metro de 5m",
                        PrecioCosto = 60.0m,
                        PrecioPublico = 110.0m,
                        PrecioMayoreo = 95.0m,
                        Existencia = 3,
                        MinStock = 5,
                        MaxStock = 25,
                        Departamento = "Herramientas",
                        Categoria = "Medición",
                        TipoVenta = "Unidad",
                        Proveedor = "Stanley",
                        FechaCreacion = DateTime.Now
                    }
                );
                
                context.SaveChanges();
                Console.WriteLine("Productos de ejemplo agregados correctamente.");
            }
            
            // Verificar y agregar clientes de ejemplo si no hay ninguno
            if (!context.Clientes.Any())
            {
                Console.WriteLine("Agregando clientes de ejemplo...");
                
                // Agregar clientes de ejemplo
                context.Clientes.AddRange(
                    new Cliente
                    {
                        Rut = "11111111-1",
                        Nombre = "Juan Pérez",
                        Email = "juanperez@mail.com",
                        Telefono = "912345678",
                        Direccion = "Avenida Principal 123",
                        Notas = "Cliente habitual",
                        FechaCreacion = DateTime.Now
                    },
                    new Cliente
                    {
                        Rut = "22222222-2",
                        Nombre = "María Gómez",
                        Email = "mariagomez@mail.com",
                        Telefono = "923456789",
                        Direccion = "Calle Secundaria 456",
                        Notas = "Cliente mayorista",
                        FechaCreacion = DateTime.Now
                    },
                    new Cliente
                    {
                        Rut = "33333333-3",
                        Nombre = "Carlos Rodríguez",
                        Email = "carlos@mail.com",
                        Telefono = "934567890",
                        Direccion = "Pasaje Los Alerces 789",
                        Notas = "",
                        FechaCreacion = DateTime.Now
                    }
                );
                
                context.SaveChanges();
                Console.WriteLine("Clientes de ejemplo agregados correctamente.");
            }
        }
    }
} 