#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Repository;

namespace GameDb.Service {
    public interface IPropertyService {
        // RealEstate
        Task<RealEstateEntity?> GetRealEstateByIdAsync(long id);
        Task<IEnumerable<RealEstateEntity>> GetRealEstatesByOwnerAsync(PlayerEntity owner);
        Task<RealEstateEntity?> GetRealEstateByAddressAsync(AddressEntity address);
        Task<bool> SetRealEstateOwnershipAsync(RealEstateEntity realEstate, long? newOwnerId);

        // Garage 
        Task<GarageEntity?> GetGarageByIdAsync(long id);
        Task<IEnumerable<GarageEntity>> GetGaragesByOwnerAsync(PlayerEntity owner);
        Task<GarageEntity?> GetGarageByAddressAsync(AddressEntity address);
        Task<bool> SetGarageOwnershipAsync(GarageEntity garage, long? newOwnerId);

        // Residence
        Task<ResidenceEntity?> RegisterResidenceAsync(PlayerEntity player, RealEstateEntity realEstate);
        Task<IEnumerable<ResidenceEntity>> GetResidencesByPlayerAsync(PlayerEntity player);
        Task<IEnumerable<ResidenceEntity>> GetResidencesByRealEstateAsync(RealEstateEntity realEstate);
        Task<bool> RemoveResidenceAsync(PlayerEntity player, RealEstateEntity realEstate);
        Task<bool> RemoveResidenceAsync(ResidenceEntity residence);

        // Address
        Task<AddressEntity?> GetAddressByIdAsync(long id);
        Task<IEnumerable<AddressEntity>> GetAddressesByNameAsync(string addressName);

        // InfrastructureBuilding
        Task<InfrastructureBuildingEntity?> GetInfrastructureBuildingByIdAsync(long id);
        Task<IEnumerable<InfrastructureBuildingEntity>> GetInfrastructureBuildingsBySocialClubAsync(SocialClubEntity socialClub);
        Task<InfrastructureBuildingEntity?> GetInfrastructureBuildingByAddressAsync(AddressEntity address);
        Task<IEnumerable<InfrastructureBuildingEntity>> GetInfrastructureBuildingsByNameAsync(string name);
    }

    public class PropertyService : IPropertyService{
        private readonly IPropertyRepository<RealEstateEntity> _realEstateRepository;
        private readonly IPropertyRepository<GarageEntity> _garageRepository;
        private readonly IResidenceRepository _residenceRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IInfrastructureBuildingRepository _infrastructureBuildingRepository;

        public PropertyService(
            IPropertyRepository<RealEstateEntity> realEstateRepository,
            IPropertyRepository<GarageEntity> garageRepository,
            IResidenceRepository residenceRepository,
            IAddressRepository addressRepository,
            IInfrastructureBuildingRepository infrastructureBuildingRepository
        ) {
            _realEstateRepository = realEstateRepository;
            _garageRepository = garageRepository;
            _residenceRepository = residenceRepository;
            _addressRepository = addressRepository;
            _infrastructureBuildingRepository = infrastructureBuildingRepository;
        }

        // RealEstate
        public async Task<RealEstateEntity?> GetRealEstateByIdAsync(long id)
        {
            var result = await _realEstateRepository.GetByIdAsync(id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving real estate by ID '{id}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<RealEstateEntity>> GetRealEstatesByOwnerAsync(PlayerEntity owner)
        {
            if (owner == null)
            {
                Console.WriteLine("Owner cannot be null.");
                return new List<RealEstateEntity>();
            }
            var result = await _realEstateRepository.GetByOwnerIdAsync(owner.Id);
            if (result.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving real estates by owner '{owner.Nickname}': {result.Message}");
                return new List<RealEstateEntity>();
            }
            return result.ReturnValue ?? new List<RealEstateEntity>();
        }

        public async Task<RealEstateEntity?> GetRealEstateByAddressAsync(AddressEntity address)
        {
            if (address == null)
            {
                Console.WriteLine("Address cannot be null.");
                return null;
            }
            var result = await _realEstateRepository.GetByAddressIdAsync(address.Id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving real estate by address '{address.AddressName}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<bool> SetRealEstateOwnershipAsync(RealEstateEntity realEstate, long? newOwnerId) {
            if (realEstate == null)
            {
                Console.WriteLine("Real estate cannot be null.");
                return false;
            }
            if (newOwnerId != null && newOwnerId <= 0)
            {
                Console.WriteLine("New owner ID must be greater than zero.");
                return false;
            }
            realEstate.OwnerId = newOwnerId;
            var saved = await _realEstateRepository.SaveChangesAsync();
            if (!saved)
            {
                Console.WriteLine("Failed to transfer real estate ownership.");
                return false;
            }
            return true;
        }

        // Garage
        public async Task<GarageEntity?> GetGarageByIdAsync(long id)
        {
            var result = await _garageRepository.GetByIdAsync(id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving garage by ID '{id}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<GarageEntity>> GetGaragesByOwnerAsync(PlayerEntity owner)
        {
            if (owner == null)
            {
                Console.WriteLine("Owner cannot be null.");
                return new List<GarageEntity>();
            }
            var result = await _garageRepository.GetByOwnerIdAsync(owner.Id);
            if (result.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving garages by owner '{owner.Nickname}': {result.Message}");
                return new List<GarageEntity>();
            }
            return result.ReturnValue ?? new List<GarageEntity>();
        }

        public async Task<GarageEntity?> GetGarageByAddressAsync(AddressEntity address)
        {
            if (address == null)
            {
                Console.WriteLine("Address cannot be null.");
                return null;
            }
            var result = await _garageRepository.GetByAddressIdAsync(address.Id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving garage by address '{address.AddressName}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<bool> SetGarageOwnershipAsync(GarageEntity garage, long? newOwnerId)
        {
            if (garage == null)
            {
                Console.WriteLine("Garage cannot be null.");
                return false;
            }
            if (newOwnerId != null && newOwnerId <= 0)
            {
                Console.WriteLine("New owner ID must be greater than zero.");
                return false;
            }
            garage.OwnerId = newOwnerId;
            var saved = await _garageRepository.SaveChangesAsync();
            if (!saved)
            {
                Console.WriteLine("Failed to transfer garage ownership.");
                return false;
            }
            return true;
        }

        // Residence
        public async Task<ResidenceEntity?> RegisterResidenceAsync(PlayerEntity player, RealEstateEntity realEstate) {
            if (player == null)
            {
                Console.WriteLine("Player cannot be null.");
                return null;
            }
            if (realEstate == null)
            {
                Console.WriteLine("Real estate cannot be null.");
                return null;
            }
            var residence = new ResidenceEntity
            {
                PlayerId = player.Id,
                RealEstateId = realEstate.Id
            };
            var result = await _residenceRepository.AddAsync(residence);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error registering residence: {result.Message}");
                return null;
            }
            var saved = await _residenceRepository.SaveChangesAsync();
            if (!saved)
            {
                Console.WriteLine("Failed to save residence.");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<ResidenceEntity>> GetResidencesByPlayerAsync(PlayerEntity player)
        {
            if (player == null)
            {
                Console.WriteLine("Player cannot be null.");
                return new List<ResidenceEntity>();
            }
            var result = await _residenceRepository.GetByPlayerIdAsync(player.Id);
            if (result.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving residences by player '{player.Nickname}': {result.Message}");
                return new List<ResidenceEntity>();
            }
            return result.ReturnValue ?? new List<ResidenceEntity>();
        }

        public async Task<IEnumerable<ResidenceEntity>> GetResidencesByRealEstateAsync(RealEstateEntity realEstate)
        {
            if (realEstate == null)
            {
                Console.WriteLine("Real estate cannot be null.");
                return new List<ResidenceEntity>();
            }
            var result = await _residenceRepository.GetByRealEstateIdAsync(realEstate.Id);
            if (result.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving residences by real estate: {result.Message}");
                return new List<ResidenceEntity>();
            }
            return result.ReturnValue ?? new List<ResidenceEntity>();
        }

        public async Task<bool> RemoveResidenceAsync(PlayerEntity player, RealEstateEntity realEstate) {
            if (player == null)
            {
                Console.WriteLine("Player cannot be null.");
                return false;
            }
            if (realEstate == null)
            {
                Console.WriteLine("Real estate cannot be null.");
                return false;
            }
            var result = await _residenceRepository.DeleteByIdAsync(player.Id, realEstate.Id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error removing residence: {result.Message}");
                return false;
            }
            var saved = await _residenceRepository.SaveChangesAsync();
            if (!saved)
            {
                Console.WriteLine("Failed to save changes after removing residence.");
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveResidenceAsync(ResidenceEntity residence)
        {
            if (residence == null)
            {
                Console.WriteLine("Residence cannot be null.");
                return false;
            }
            var result = await _residenceRepository.DeleteByIdAsync(residence.PlayerId, residence.RealEstateId);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error removing residence: {result.Message}");
                return false;
            }
            var saved = await _residenceRepository.SaveChangesAsync();
            if (!saved)
            {
                Console.WriteLine("Failed to save changes after removing residence.");
                return false;
            }
            return true;
        }

        // Address
        public async Task<AddressEntity?> GetAddressByIdAsync(long id)
        {
            if (id <= 0)
            {
                Console.WriteLine("Address ID must be greater than zero.");
                return null;
            }
            var result = await _addressRepository.GetByIdAsync(id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving address by ID '{id}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<AddressEntity>> GetAddressesByNameAsync(string addressName)
        {
            if (string.IsNullOrWhiteSpace(addressName))
            {
                Console.WriteLine("Address name cannot be null or empty.");
                return new List<AddressEntity>();
            }
            var result = await _addressRepository.GetByAddressNameAsync(addressName);
            if (result.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving addresses by name '{addressName}': {result.Message}");
                return new List<AddressEntity>();
            }
            return result.ReturnValue ?? new List<AddressEntity>();
        }

        // InfrastructureBuilding
        public async Task<InfrastructureBuildingEntity?> GetInfrastructureBuildingByIdAsync(long id)
        {
            if (id <= 0)
            {
                Console.WriteLine("Infrastructure building ID must be greater than zero.");
                return null;
            }
            var result = await _infrastructureBuildingRepository.GetByIdAsync(id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving infrastructure building by ID '{id}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<InfrastructureBuildingEntity>> GetInfrastructureBuildingsBySocialClubAsync(SocialClubEntity socialClub)
        {
            if (socialClub == null)
            {
                Console.WriteLine("Social club cannot be null.");
                return new List<InfrastructureBuildingEntity>();
            }
            var result = await _infrastructureBuildingRepository.GetBySocialClubIdAsync(socialClub.Id);
            if (result.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving infrastructure buildings by social club '{socialClub.Name}': {result.Message}");
                return new List<InfrastructureBuildingEntity>();
            }
            return result.ReturnValue ?? new List<InfrastructureBuildingEntity>();
        }

        public async Task<InfrastructureBuildingEntity?> GetInfrastructureBuildingByAddressAsync(AddressEntity address)
        {
            if (address == null)
            {
                Console.WriteLine("Address cannot be null.");
                return null;
            }
            var result = await _infrastructureBuildingRepository.GetByAddressIdAsync(address.Id);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving infrastructure building by address '{address.AddressName}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<IEnumerable<InfrastructureBuildingEntity>> GetInfrastructureBuildingsByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Infrastructure building name cannot be null or empty.");
                return new List<InfrastructureBuildingEntity>();
            }
            var result = await _infrastructureBuildingRepository.GetByNameAsync(name);
            if (result.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error retrieving infrastructure buildings by name '{name}': {result.Message}");
                return new List<InfrastructureBuildingEntity>();
            }
            return result.ReturnValue ?? new List<InfrastructureBuildingEntity>();
        }
    }
}