// ProductosHexagonal.Infrastructure/Data/ProductosDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProductosHexagonal.Domain.Models;

namespace ProductosHexagonal.Infrastructure.Data
{
    public class ProductosDbContext : DbContext
    {
        public ProductosDbContext(DbContextOptions<ProductosDbContext> options)
            : base(options)
        {
        }

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();
                
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Precio)
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
                
                entity.Property(e => e.Stock)
                    .IsRequired();
                
                entity.Property(e => e.FechaCreacion)
                    .IsRequired();
                
                entity.Property(e => e.Activo)
                    .IsRequired()
                    .HasDefaultValue(true);

                // Índice para búsquedas por nombre
                entity.HasIndex(e => e.Nombre);

                // Datos semilla
                entity.HasData(
                    new Producto 
                    { 
                        Id = 1, 
                        Nombre = "Laptop Dell XPS 13", 
                        Descripcion = "Ultrabook de alto rendimiento", 
                        Precio = 1299.99m, 
                        Stock = 10,
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    },
                    new Producto 
                    { 
                        Id = 2, 
                        Nombre = "Mouse Logitech MX Master 3", 
                        Descripcion = "Mouse ergonómico inalámbrico", 
                        Precio = 99.99m, 
                        Stock = 25,
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    },
                    new Producto 
                    { 
                        Id = 3, 
                        Nombre = "Teclado Mecánico Keychron K2", 
                        Descripcion = "Teclado mecánico RGB bluetooth", 
                        Precio = 89.99m, 
                        Stock = 15,
                        FechaCreacion = DateTime.Now,
                        Activo = true
                    }
                );
            });
        }
    }
}
