using Microsoft.EntityFrameworkCore;
using GameDb.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;

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
        public DbSet<ResidenceEntity> Residences { get; set; }
        public DbSet<InfrastructureBuildingEntity> InfrastructureBuildings { get; set; }
        public DbSet<SocialClubEntity> SocialClubs { get; set; }
        public DbSet<GarageEntity> Garages { get; set; }
        public DbSet<PunishmentEntity> Punishments { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }

        public GameDbContext() {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            _connectionString = configuration.GetConnectionString("PostgresConnection");

            if (string.IsNullOrEmpty(_connectionString)) {
                throw new InvalidOperationException("Connection string 'PostgresConnection' not found in appsettings.json.");
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player -> Vehicle 1-Many
            modelBuilder.Entity<PlayerEntity>()
                .HasMany(p => p.Vehicles)
                .WithOne(v => v.Owner)
                .HasForeignKey(v => v.OwnerId)
                .IsRequired(false);

            // Player -> RealEstate 1-Many
            modelBuilder.Entity<PlayerEntity>()
                .HasMany(p => p.RealEstates)
                .WithOne(re => re.Owner)
                .HasForeignKey(re => re.OwnerId)
                .IsRequired(false);

            // Address -> RealEstate 1-Many
            modelBuilder.Entity<AddressEntity>()
                .HasMany(a => a.RealEstates)
                .WithOne(re => re.Address)
                .HasForeignKey(re => re.AddressId)
                .IsRequired();

            // Ensure AddressId is unique in RealEstateEntity for the One-to-One relationship
            modelBuilder.Entity<RealEstateEntity>()
                .HasIndex(re => re.AddressId)
                .IsUnique();

            modelBuilder.Entity<InventoryEntity>()
                .Property(i => i.Items)
                .HasColumnType("jsonb");
            modelBuilder.Entity<InventoryEntity>()
                .Property(i => i.Cells)
                .HasColumnType("jsonb");

            // Player -> Inventory 1-1
            modelBuilder.Entity<PlayerEntity>()
                .HasOne(p => p.Inventory)
                .WithOne(i => i.Player)
                .HasForeignKey<InventoryEntity>(i => i.PlayerId)
                .IsRequired();

            // Player -> Residence 1-1
            modelBuilder.Entity<PlayerEntity>()
                .HasOne(p => p.Residence)
                .WithOne(r => r.Player)
                .HasForeignKey<ResidenceEntity>(r => r.PlayerId)
                .IsRequired();

            // RealEstate -> Residence 1-Many
            modelBuilder.Entity<RealEstateEntity>()
                .HasMany(re => re.Residences)
                .WithOne(r => r.RealEstate)
                .HasForeignKey(r => r.RealEstateId)
                .IsRequired();

            // SocialClub -> Player 1-Many
            modelBuilder.Entity<SocialClubEntity>()
                .HasMany(s => s.Players)
                .WithOne(p => p.SocialClub)
                .HasForeignKey(p => p.SocialClubId)
                .IsRequired(false);

            // SocialClub -> InfrastructureBuilding 1-Many
            modelBuilder.Entity<SocialClubEntity>()
                .HasMany(s => s.InfrastructureBuildings)
                .WithOne(b => b.SocialClub)
                .HasForeignKey(b => b.SocialClubId)
                .IsRequired(false);

            // Address -> InfrastructureBuilding 1-1
            modelBuilder.Entity<AddressEntity>()
                .HasOne(a => a.InfrastructureBuilding)
                .WithOne(b => b.Address)
                .HasForeignKey<InfrastructureBuildingEntity>(b => b.AddressId)
                .IsRequired();

            // Role -> Player 1-Many
            modelBuilder.Entity<RoleEntity>()
                .HasMany(r => r.Players)
                .WithOne(p => p.Role)
                .HasForeignKey(p => p.RoleId)
                .IsRequired();

            // Player -> Punishment 1-Many
            modelBuilder.Entity<PlayerEntity>()
                .HasMany(p => p.Punishments)
                .WithOne(pu => pu.Player)
                .HasForeignKey(pu => pu.PlayerId)
                .IsRequired();

            // Player -> Garage 1-Many
            modelBuilder.Entity<PlayerEntity>()
                .HasMany(p => p.Garages)
                .WithOne(g => g.Owner)
                .HasForeignKey(g => g.OwnerId)
                .IsRequired(false);

            // Explicit primary keys for safety
            modelBuilder.Entity<PlayerEntity>().HasKey(p => p.Id);
            modelBuilder.Entity<PlayerEntity>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<PlayerEntity>()
                .HasIndex(p => p.Nickname)
                .IsUnique();
            modelBuilder.Entity<PlayerEntity>()
                .Property(p => p.Position)
                .HasColumnType("jsonb");
            modelBuilder.Entity<PlayerEntity>()
                .Property(p => p.RoleId)
                .IsRequired();
            modelBuilder.Entity<VehicleEntity>().HasKey(v => v.Id);
            modelBuilder.Entity<VehicleEntity>()
                .Property(v => v.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<VehicleEntity>()
                .Property(v => v.Position)
                .HasColumnType("jsonb");
            modelBuilder.Entity<RealEstateEntity>().HasKey(re => re.Id);
            modelBuilder.Entity<RealEstateEntity>()
                .Property(re => re.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<RealEstateEntity>()
                .Property(re => re.SpawnPoint)
                .HasColumnType("jsonb");
            modelBuilder.Entity<GarageEntity>().HasKey(g => g.Id);
            modelBuilder.Entity<GarageEntity>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<GarageEntity>()
                .Property(g => g.VehicleSpawnPoints)
                .HasColumnType("jsonb");
            modelBuilder.Entity<AddressEntity>().HasKey(a => a.Id);
            modelBuilder.Entity<AddressEntity>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<ItemEntity>().HasKey(i => i.Id);
            modelBuilder.Entity<ItemEntity>()
                .Property(i => i.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<InventoryEntity>().HasKey(i => i.PlayerId);
            modelBuilder.Entity<ResidenceEntity>().HasKey(r => new { r.PlayerId, r.RealEstateId });
            modelBuilder.Entity<InfrastructureBuildingEntity>().HasKey(b => b.Id);
            modelBuilder.Entity<InfrastructureBuildingEntity>()
                .Property(b => b.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<SocialClubEntity>().HasKey(s => s.Id);
            modelBuilder.Entity<SocialClubEntity>()
                .Property(s => s.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<GarageEntity>().HasKey(g => g.Id);
            modelBuilder.Entity<GarageEntity>()
                .Property(g => g.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<GarageEntity>()
                .Property(g => g.VehicleSpawnPoints)
                .HasColumnType("jsonb");
            modelBuilder.Entity<RoleEntity>().HasKey(r => r.Id);
            modelBuilder.Entity<RoleEntity>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<RoleEntity>()
                .Property(r => r.Permissions)
                .HasColumnType("jsonb");
            modelBuilder.Entity<ItemEntity>()
                .Property(i => i.DefaultCells)
                .HasColumnType("jsonb");
            modelBuilder.Entity<ItemEntity>()
                .Property(i => i.Usage)
                .HasColumnType("jsonb");
            modelBuilder.Entity<PunishmentEntity>().HasKey(pu => pu.Id);
            modelBuilder.Entity<PunishmentEntity>()
                .Property(pu => pu.Id)
                .ValueGeneratedOnAdd();
            
        }
    }
}