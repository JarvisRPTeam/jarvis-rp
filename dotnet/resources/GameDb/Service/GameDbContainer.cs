using System;
using GameDb.Domain.Entities;
using GameDb.Repository;

namespace GameDb.Service {
    public static class GameDbContainer {
        private static readonly GameDbContext _context;
        private static readonly IGameDbRepository<PlayerEntity> _playerRepository;
        private static readonly IGameDbRepository<VehicleEntity> _vehicleRepository;
        private static readonly IGameDbRepository<SocialClubEntity> _socialClubRepository;
        private static readonly IGameDbRepository<RealEstateEntity> _realEstateRepository;
        private static readonly IGameDbRepository<ItemEntity> _itemRepository;
        private static readonly IGameDbRepository<InventoryEntity> _inventoryRepository;
        private static readonly IGameDbRepository<AddressEntity> _addressRepository;
        private static readonly IGameDbRepository<InfrastructureBuildingEntity> _infrastructureBuildingRepository;
        private static readonly IGameDbRepository<ResidenceEntity> _residenceRepository;

        public static IPlayerService PlayerService { get; }
        public static IVehicleService VehicleService { get; }
        public static IInventoryService InventoryService { get; }

        static GameDbContainer() {
            try {
                _context = new GameDbContext();

                _socialClubRepository = new GameDbRepository<SocialClubEntity>(_context);
                _playerRepository = new GameDbRepository<PlayerEntity>(_context);
                _vehicleRepository = new GameDbRepository<VehicleEntity>(_context);
                _realEstateRepository = new GameDbRepository<RealEstateEntity>(_context);
                _itemRepository = new GameDbRepository<ItemEntity>(_context);
                _inventoryRepository = new GameDbRepository<InventoryEntity>(_context);
                _addressRepository = new GameDbRepository<AddressEntity>(_context);
                _infrastructureBuildingRepository = new GameDbRepository<InfrastructureBuildingEntity>(_context);
                _residenceRepository = new GameDbRepository<ResidenceEntity>(_context);

                PlayerService = new PlayerService(_playerRepository, _socialClubRepository);
                VehicleService = new VehicleService(_vehicleRepository, _context);
                InventoryService = new InventoryService(_inventoryRepository, _context);
            } catch (Exception ex) {
                Console.WriteLine($"Error initializing GameDbContainer: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new InvalidOperationException("GameDbContainer initialization failed.", ex);
            }
        }

        public static bool IsReady() {
            return _context != null && _playerRepository != null && _vehicleRepository != null &&
                   _socialClubRepository != null && _realEstateRepository != null &&
                   _itemRepository != null && _inventoryRepository != null &&
                   _addressRepository != null && _infrastructureBuildingRepository != null &&
                   _residenceRepository != null;
        }
    }
}