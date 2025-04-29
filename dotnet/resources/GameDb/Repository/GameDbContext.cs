using Microsoft.EntityFrameworkCore;
using GameDb.Domain;

namespace GameDb.Repository
{
    public class GameDbContext : DbContext {
        private readonly string _connectionString;

        public DbSet<PlayerEntity> Players { get; set; }
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
            base.OnModelCreating(modelBuilder); // Call the base method

            // Configure Player -> Vehicle relationship (One-to-Many)
            modelBuilder.Entity<PlayerEntity>()
                .HasMany<VehicleEntity>() // Player has many Vehicles
                .WithOne() // Vehicle has one Owner (Player)
                .HasForeignKey(v => v.OwnerId) // Foreign key in VehicleEntity
                .IsRequired(false); // OwnerId is nullable (optional owner)

            // Configure Player -> RealEstate relationship (One-to-Many)
            modelBuilder.Entity<PlayerEntity>()
                .HasMany<RealEstateEntity>() // Player has many RealEstates
                .WithOne() // RealEstate has one Owner (Player)
                .HasForeignKey(re => re.OwnerId) // Foreign key in RealEstateEntity
                .IsRequired(false); // OwnerId is nullable (optional owner)

            // Configure Address -> RealEstate relationship (One-to-One)
            // Assuming one Address corresponds to exactly one RealEstate
            modelBuilder.Entity<AddressEntity>()
                .HasOne<RealEstateEntity>() // Address has one RealEstate
                .WithOne() // RealEstate has one Address
                .HasForeignKey<RealEstateEntity>(re => re.AddressId) // Foreign key is in RealEstateEntity
                .IsRequired(); // AddressId is not nullable (required address)

            // Ensure AddressId is unique in RealEstateEntity for the One-to-One relationship
            modelBuilder.Entity<RealEstateEntity>()
                .HasIndex(re => re.AddressId)
                .IsUnique();

            // Configure primary keys explicitly for safety
            modelBuilder.Entity<PlayerEntity>().HasKey(p => p.Id);
            modelBuilder.Entity<VehicleEntity>().HasKey(v => v.Id);
            modelBuilder.Entity<RealEstateEntity>().HasKey(re => re.Id);
            modelBuilder.Entity<AddressEntity>().HasKey(a => a.Id);
        }
    }
}