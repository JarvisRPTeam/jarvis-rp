using Microsoft.EntityFrameworkCore;
using GameDb.Domain.Entities;

namespace GameDb.Repository
{
    public class GameDbContext : DbContext {
        private readonly string _connectionString;

        public DbSet<PlayerEntity> Players { get; set; }
        public DbSet<InventoryEntity> Inventories { get; set; }
        public DbSet<ItemEntity> Items { get; set; }
        public DbSet<VehicleEntity> Vehicles { get; set; }
        public DbSet<RealEstateEntity> RealEstates { get; set; }
        public DbSet<AddressEntity> Addresses { get; set; }

        public GameDbContext(string connectionString) {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // Player -> Vehicle 1-Many
            modelBuilder.Entity<PlayerEntity>()
                .HasMany<VehicleEntity>()
                .WithOne()
                .HasForeignKey(v => v.OwnerId)
                .IsRequired(false);

            // Player -> RealEstate 1-Many
            modelBuilder.Entity<PlayerEntity>()
                .HasMany<RealEstateEntity>()
                .WithOne()
                .HasForeignKey(re => re.OwnerId)
                .IsRequired(false);

            // Address -> RealEstate 1-1
            modelBuilder.Entity<AddressEntity>()
                .HasOne<RealEstateEntity>()
                .WithOne()
                .HasForeignKey<RealEstateEntity>(re => re.AddressId)
                .IsRequired();

            // Ensure AddressId is unique in RealEstateEntity for the One-to-One relationship
            modelBuilder.Entity<RealEstateEntity>()
                .HasIndex(re => re.AddressId)
                .IsUnique();

            modelBuilder.Entity<InventoryEntity>()
                .Property(i => i.Items)
                .HasColumnType("jsonb");

            // Player -> Inventory 1-1
            modelBuilder.Entity<PlayerEntity>()
                .HasOne<InventoryEntity>()
                .WithOne()
                .HasForeignKey<InventoryEntity>(i => i.PlayerId)
                .IsRequired();

            // Explicit primary keys for safety
            modelBuilder.Entity<PlayerEntity>().HasKey(p => p.Id);
            modelBuilder.Entity<VehicleEntity>().HasKey(v => v.Id);
            modelBuilder.Entity<RealEstateEntity>().HasKey(re => re.Id);
            modelBuilder.Entity<AddressEntity>().HasKey(a => a.Id);
            modelBuilder.Entity<ItemEntity>().HasKey(i => i.Id);
            modelBuilder.Entity<InventoryEntity>().HasKey(i => i.PlayerId);
        }
    }
}