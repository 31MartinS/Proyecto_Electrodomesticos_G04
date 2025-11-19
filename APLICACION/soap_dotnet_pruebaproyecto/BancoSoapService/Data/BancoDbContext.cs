using Microsoft.EntityFrameworkCore;
using BancoSoapService.Models;

namespace BancoSoapService.Data
{
    public class BancoDbContext : DbContext
    {
        public BancoDbContext(DbContextOptions<BancoDbContext> options) : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; } = null!;
        public DbSet<Cuenta> Cuentas { get; set; } = null!;
        public DbSet<Movimiento> Movimientos { get; set; } = null!;
        public DbSet<Credito> Creditos { get; set; } = null!;
        public DbSet<CuotaCredito> CuotasCredito { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Índices únicos
            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Cedula)
                .IsUnique();

            modelBuilder.Entity<Cuenta>()
                .HasIndex(c => c.NumeroCuenta)
                .IsUnique();

            // Configuración de relaciones
            modelBuilder.Entity<Cuenta>()
                .HasOne(c => c.Cliente)
                .WithMany(cl => cl.Cuentas)
                .HasForeignKey(c => c.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Movimiento>()
                .HasOne(m => m.Cuenta)
                .WithMany(c => c.Movimientos)
                .HasForeignKey(m => m.IdCuenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Credito>()
                .HasOne(cr => cr.Cliente)
                .WithMany(cl => cl.Creditos)
                .HasForeignKey(cr => cr.IdCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Credito>()
                .HasOne(cr => cr.Cuenta)
                .WithMany(c => c.Creditos)
                .HasForeignKey(cr => cr.IdCuenta)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CuotaCredito>()
                .HasOne(cc => cc.Credito)
                .WithMany(cr => cr.Cuotas)
                .HasForeignKey(cc => cc.IdCredito)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
