using FerreteriaPOS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace FerreteriaPOS.Data
{
    public class FerreteriaContext : DbContext
    {
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaItem> VentaItems { get; set; }
        public DbSet<Configuracion> Configuraciones { get; set; }

        // Constructor para pruebas con InMemory
        public FerreteriaContext(DbContextOptions<FerreteriaContext> options) : base(options)
        {
        }
        
        // Constructor sin argumentos para uso normal
        public FerreteriaContext() : base()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Si ya está configurado (por ejemplo, en pruebas), no hacemos nada
            if (optionsBuilder.IsConfigured)
                return;
                
            // Asegurarnos de que el directorio de datos existe
            string directorioDatos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FerreteriaPOS");
            if (!Directory.Exists(directorioDatos))
            {
                Directory.CreateDirectory(directorioDatos);
            }
            
            // Ruta de la base de datos
            string rutaBaseDatos = Path.Combine(directorioDatos, "ferreteria.db");
            
            // Conectar a SQLite
            optionsBuilder.UseSqlite($"Data Source={rutaBaseDatos}");
            
            // Habilitar logging detallado en modo debug
            optionsBuilder.EnableSensitiveDataLogging();
            
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuraciones específicas de modelo
            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.CodigoBarra)
                .IsUnique();
                
            // Explícitamente hacer campos problemáticos de Producto como nullable
            modelBuilder.Entity<Producto>()
                .Property(p => p.Categoria)
                .IsRequired(false);
                
            modelBuilder.Entity<Producto>()
                .Property(p => p.TipoVenta)
                .IsRequired(false);
                
            modelBuilder.Entity<Producto>()
                .Property(p => p.Proveedor)
                .IsRequired(false);
                
            modelBuilder.Entity<Producto>()
                .Property(p => p.Departamento)
                .IsRequired(false);
                
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => new { c.Rut, c.Nombre })
                .IsUnique();
                
            // Explícitamente hacer campos problemáticos de Cliente como nullable
            modelBuilder.Entity<Cliente>()
                .Property(c => c.Email)
                .IsRequired(false);
                
            modelBuilder.Entity<Cliente>()
                .Property(c => c.Telefono)
                .IsRequired(false);
                
            modelBuilder.Entity<Cliente>()
                .Property(c => c.Direccion)
                .IsRequired(false);
                
            modelBuilder.Entity<Cliente>()
                .Property(c => c.Notas)
                .IsRequired(false);
                
            // Explícitamente hacer campos problemáticos de Venta como nullable
            modelBuilder.Entity<Venta>()
                .Property(v => v.NumeroFactura)
                .IsRequired(false);
                
            modelBuilder.Entity<Venta>()
                .Property(v => v.EstadoFactura)
                .IsRequired(false);
                
            modelBuilder.Entity<Venta>()
                .Property(v => v.Usuario)
                .IsRequired(false);
                
            modelBuilder.Entity<Venta>()
                .Property(v => v.Observaciones)
                .IsRequired(false);
                
            // Configuraciones de relaciones
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Cliente)
                .WithMany(c => c.Ventas)
                .HasForeignKey(v => v.ClienteId)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<VentaItem>()
                .HasOne(vi => vi.Producto)
                .WithMany()
                .HasForeignKey(vi => vi.ProductoId)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<VentaItem>()
                .HasOne(vi => vi.Venta)
                .WithMany(v => v.Items)
                .HasForeignKey(vi => vi.VentaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
        
        // Método para recrear la base de datos (útil en desarrollo)
        public void RecreateDatabase()
        {
            // Eliminar y recrear la base de datos
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
} 