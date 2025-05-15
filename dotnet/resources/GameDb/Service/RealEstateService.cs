using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Repository;

namespace GameDb.Service {
    public interface IRealEstateService {
        // RealEstate
        Task<DbQueryResult<RealEstateEntity>> GetRealEstateByIdAsync(long id);
        Task<DbQueryResult<IEnumerable<RealEstateEntity>>> GetRealEstatesByOwnerIdAsync(long ownerId);
        Task<DbQueryResult<RealEstateEntity>> GetRealEstateByAddressIdAsync(long addressId);
        Task<DbQueryResult<RealEstateEntity>> TransferOwnershipAsync(long realEstateId, long? newOwnerId);

        // Residence
        Task<DbQueryResult<ResidenceEntity>> RegisterResidenceAsync(long playerId, long realEstateId);
        Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetResidencesByPlayerIdAsync(long playerId);
        Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetResidencesByRealEstateIdAsync(long realEstateId);
        Task<DbQueryResult<ResidenceEntity>> RemoveResidenceAsync(long playerId, long realEstateId);

        // Address
        Task<DbQueryResult<AddressEntity>> GetAddressByIdAsync(long id);
        Task<DbQueryResult<IEnumerable<AddressEntity>>> GetAddressesByNameAsync(string addressName);

        // InfrastructureBuilding
        Task<DbQueryResult<InfrastructureBuildingEntity>> GetInfrastructureBuildingByIdAsync(long id);
        Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetInfrastructureBuildingsBySocialClubIdAsync(long socialClubId);
        Task<DbQueryResult<InfrastructureBuildingEntity>> GetInfrastructureBuildingByAddressIdAsync(long addressId);
        Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetInfrastructureBuildingsByNameAsync(string name);
    }

    public class RealEstateService : IRealEstateService {
        private readonly IRealEstateRepository _realEstateRepository;
        private readonly IResidenceRepository _residenceRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IInfrastructureBuildingRepository _infrastructureBuildingRepository;

        public RealEstateService(
            IRealEstateRepository realEstateRepository,
            IResidenceRepository residenceRepository,
            IAddressRepository addressRepository,
            IInfrastructureBuildingRepository infrastructureBuildingRepository
        ) {
            _realEstateRepository = realEstateRepository;
            _residenceRepository = residenceRepository;
            _addressRepository = addressRepository;
            _infrastructureBuildingRepository = infrastructureBuildingRepository;
        }

        // RealEstate
        public Task<DbQueryResult<RealEstateEntity>> GetRealEstateByIdAsync(long id)
            => _realEstateRepository.GetByIdAsync(id);

        public Task<DbQueryResult<IEnumerable<RealEstateEntity>>> GetRealEstatesByOwnerIdAsync(long ownerId)
            => _realEstateRepository.GetByOwnerIdAsync(ownerId);

        public Task<DbQueryResult<RealEstateEntity>> GetRealEstateByAddressIdAsync(long addressId)
            => _realEstateRepository.GetByAddressIdAsync(addressId);

        public async Task<DbQueryResult<RealEstateEntity>> TransferOwnershipAsync(long realEstateId, long? newOwnerId) {
            var result = await _realEstateRepository.GetByIdAsync(realEstateId);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
                return new DbQueryResult<RealEstateEntity>(DbResultType.Warning, "Real estate not found.");
            result.ReturnValue.OwnerId = newOwnerId;
            var saved = await _realEstateRepository.SaveChangesAsync();
            if (!saved) return new DbQueryResult<RealEstateEntity>(DbResultType.Error, "Failed to transfer ownership.");
            return new DbQueryResult<RealEstateEntity>(DbResultType.Success, "Ownership transferred.", result.ReturnValue);
        }

        // Residence
        public async Task<DbQueryResult<ResidenceEntity>> RegisterResidenceAsync(long playerId, long realEstateId) {
            var entity = new ResidenceEntity { PlayerId = playerId, RealEstateId = realEstateId };
            var addResult = await _residenceRepository.AddAsync(entity);
            if (addResult.ResultType != DbResultType.Success) return addResult;
            var saved = await _residenceRepository.SaveChangesAsync();
            if (!saved) return new DbQueryResult<ResidenceEntity>(DbResultType.Error, "Failed to save residence.");
            return new DbQueryResult<ResidenceEntity>(DbResultType.Success, "Residence registered.", entity);
        }

        public Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetResidencesByPlayerIdAsync(long playerId)
            => _residenceRepository.GetByPlayerIdAsync(playerId);

        public Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetResidencesByRealEstateIdAsync(long realEstateId)
            => _residenceRepository.GetByRealEstateIdAsync(realEstateId);

        public async Task<DbQueryResult<ResidenceEntity>> RemoveResidenceAsync(long playerId, long realEstateId) {
            var entity = await _residenceRepository.GetByIdAsync(playerId, realEstateId);
            if (entity.ResultType != DbResultType.Success || entity.ReturnValue == null)
            {
                return new DbQueryResult<ResidenceEntity>(DbResultType.Warning, "Residence not found.");
            } 
            var deleteResult = await _residenceRepository.DeleteByIdAsync(playerId, realEstateId);
            if (deleteResult.ResultType != DbResultType.Success)
            {
                return new DbQueryResult<ResidenceEntity>(DbResultType.Error, "Failed to remove residence.");
            }
            var saved = await _residenceRepository.SaveChangesAsync();
            if (!saved) return new DbQueryResult<ResidenceEntity>(DbResultType.Error, "Failed to save changes.");
            return new DbQueryResult<ResidenceEntity>(DbResultType.Success, "Residence removed.", entity.ReturnValue);
        }

        // Address
        public Task<DbQueryResult<AddressEntity>> GetAddressByIdAsync(long id)
            => _addressRepository.GetByIdAsync(id);

        public Task<DbQueryResult<IEnumerable<AddressEntity>>> GetAddressesByNameAsync(string addressName)
            => _addressRepository.GetByAddressNameAsync(addressName);

        // InfrastructureBuilding
        public Task<DbQueryResult<InfrastructureBuildingEntity>> GetInfrastructureBuildingByIdAsync(long id)
            => _infrastructureBuildingRepository.GetByIdAsync(id);

        public Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetInfrastructureBuildingsBySocialClubIdAsync(long socialClubId)
            => _infrastructureBuildingRepository.GetBySocialClubIdAsync(socialClubId);

        public Task<DbQueryResult<InfrastructureBuildingEntity>> GetInfrastructureBuildingByAddressIdAsync(long addressId)
            => _infrastructureBuildingRepository.GetByAddressIdAsync(addressId);

        public Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetInfrastructureBuildingsByNameAsync(string name)
            => _infrastructureBuildingRepository.GetByNameAsync(name);
    }
}