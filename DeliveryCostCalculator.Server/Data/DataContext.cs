using DeliveryCostCalculator.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
      
    }

    public DbSet<Country> Country { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryService> DeliveryServices { get; set; }
    public DbSet<DeliveryServiceProperties> DeliveryServiceProperties { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Delivery>()
            .HasOne(e => e.Country)
            .WithMany(e => e.Delivery)
            .HasForeignKey(e => e.CountryId)
            .IsRequired();

        modelBuilder.Entity<Delivery>()
            .HasOne(e => e.DeliveryService)
            .WithMany(e => e.Delivery)
            .HasForeignKey(e => e.DeliveryServiceId)
            .IsRequired();

        modelBuilder.Entity<DeliveryServiceProperties>()
            .HasOne(e => e.DeliveryService)
            .WithMany(e => e.DeliveryServiceProperties)
            .HasForeignKey(e => e.DeliveryServiceId)
            .IsRequired();


        modelBuilder.Seed();
    }
}