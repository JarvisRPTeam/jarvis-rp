using System;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Repository;

namespace GameDb.Service {
    public static class GameDbContainer {
        private static readonly GameDbContext _context;
        private static readonly IPlayerRepository _playerRepository;
        private static readonly IVehicleRepository _vehicleRepository;
        private static readonly ISocialClubRepository _socialClubRepository;
        private static readonly IPropertyRepository<RealEstateEntity> _realEstateRepository;
        private static readonly IPropertyRepository<GarageEntity> _garageRepository;
        private static readonly IItemRepository _itemRepository;
        private static readonly IInventoryRepository _inventoryRepository;
        private static readonly IAddressRepository _addressRepository;
        private static readonly IInfrastructureBuildingRepository _infrastructureBuildingRepository;
        private static readonly IResidenceRepository _residenceRepository;
        private static readonly IRoleRepository _roleRepository;
        private static readonly IPunishmentRepository _punishmentRepository;

        public static IPlayerService PlayerService { get; }
        public static IVehicleService VehicleService { get; }
        public static IInventoryService InventoryService { get; }
        public static IPropertyService PropertyService { get; }

        static GameDbContainer()
        {
            try
            {
                _context = new GameDbContext();

                _socialClubRepository = new SocialClubRepository(_context);
                _playerRepository = new PlayerRepository(_context);
                _vehicleRepository = new VehicleRepository(_context);
                _realEstateRepository = new RealEstateRepository(_context);
                _garageRepository = new GarageRepository(_context);
                _itemRepository = new ItemRepository(_context);
                _inventoryRepository = new InventoryRepository(_context);
                _addressRepository = new AddressRepository(_context);
                _infrastructureBuildingRepository = new InfrastructureBuildingRepository(_context);
                _residenceRepository = new ResidenceRepository(_context);
                _roleRepository = new RoleRepository(_context);
                _punishmentRepository = new PunishmentRepository(_context);

                PlayerService = new PlayerService(_playerRepository, _socialClubRepository, _inventoryRepository, _roleRepository, _punishmentRepository, _context);
                VehicleService = new VehicleService(_vehicleRepository, _context);
                InventoryService = new InventoryService(_inventoryRepository, _itemRepository, _context);
                PropertyService = new PropertyService(_realEstateRepository, _garageRepository, _residenceRepository, _addressRepository, _infrastructureBuildingRepository);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing GameDbContainer: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw new InvalidOperationException("GameDbContainer initialization failed.", ex);
            }
        }

        public static async Task<bool> IsReady()
        {
            try
            {
                var initialized = _context != null && _playerRepository != null && _vehicleRepository != null &&
                   _socialClubRepository != null && _realEstateRepository != null && _garageRepository != null &&
                   _itemRepository != null && _inventoryRepository != null &&
                   _addressRepository != null && _infrastructureBuildingRepository != null &&
                   _residenceRepository != null && _roleRepository != null && _punishmentRepository != null;

                if (!initialized)
                {
                    return false;
                }

                var rolesExist = await PlayerService.GetRoleByNameAsync("Player") != null &&
                                await PlayerService.GetRoleByNameAsync("Admin") != null &&
                                await PlayerService.GetRoleByNameAsync("ServerOwner") != null;

                return rolesExist;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking GameDbContainer readiness: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return false;
            }            
        }
    }
}