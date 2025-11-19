using Microsoft.EntityFrameworkCore;
using ComercializadoraAPI.Models;

namespace ComercializadoraAPI.Data
{
    public class ComercializadoraDbContext : DbContext
    {
        public ComercializadoraDbContext(DbContextOptions<ComercializadoraDbContext> options) : base(options)
        {
        }

        public DbSet<ClienteComercializadora> Clientes { get; set; } = null!;
        public DbSet<Producto> Productos { get; set; } = null!;
        public DbSet<Factura> Facturas { get; set; } = null!;
        public DbSet<DetalleFactura> DetallesFactura { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Índices únicos
            modelBuilder.Entity<ClienteComercializadora>()
                .HasIndex(c => c.Cedula)
                .IsUnique();

            modelBuilder.Entity<Producto>()
                .HasIndex(p => p.Codigo)
                .IsUnique();

            modelBuilder.Entity<Factura>()
                .HasIndex(f => f.NumeroFactura)
                .IsUnique();

            // Configuración de relaciones
            modelBuilder.Entity<Factura>()
                .HasOne(f => f.Cliente)
                .WithMany(c => c.Facturas)
                .HasForeignKey(f => f.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(df => df.Factura)
                .WithMany(f => f.Detalles)
                .HasForeignKey(df => df.IdFactura)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleFactura>()
                .HasOne(df => df.Producto)
                .WithMany(p => p.DetallesFactura)
                .HasForeignKey(df => df.IdProducto)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
