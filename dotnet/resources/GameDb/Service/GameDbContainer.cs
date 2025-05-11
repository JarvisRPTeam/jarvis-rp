using System;
using GameDb.Domain.Entities;
using GameDb.Repository;

namespace GameDb.Service {
    public static class GameDbContainer {
        private static readonly GameDbContext _context;
        private static readonly IPlayerRepository _playerRepository;
        private static readonly IVehicleRepository _vehicleRepository;
        private static readonly ISocialClubRepository _socialClubRepository;
        private static readonly IRealEstateRepository _realEstateRepository;
        private static readonly IItemRepository _itemRepository;
        private static readonly IGameDbRepository<InventoryEntity> _inventoryRepository;
        private static readonly IAddressRepository _addressRepository;
        private static readonly IInfrastructureBuildingRepository _infrastructureBuildingRepository;
        private static readonly IResidenceRepository _residenceRepository;

        public static IPlayerService PlayerService { get; }
        public static IVehicleService VehicleService { get; }
        public static IInventoryService InventoryService { get; }

        static GameDbContainer() {
            try {
                _context = new GameDbContext();

                _socialClubRepository = new SocialClubRepository(_context);
                _playerRepository = new PlayerRepository(_context);
                _vehicleRepository = new VehicleRepository(_context);
                _realEstateRepository = new RealEstateRepository(_context);
                _itemRepository = new ItemRepository(_context);
                _inventoryRepository = new GameDbRepository<InventoryEntity>(_context);
                _addressRepository = new AddressRepository(_context);
                _infrastructureBuildingRepository = new InfrastructureBuildingRepository(_context);
                _residenceRepository = new ResidenceRepository(_context);

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