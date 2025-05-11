using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository {
    public interface IRealEstateRepository: IGameDbRepository<RealEstateEntity> {
        Task<DbQueryResult<IEnumerable<RealEstateEntity>>> GetByOwnerIdAsync(long ownerId);
        Task<DbQueryResult<RealEstateEntity>> GetByAddressIdAsync(long addressId);
    }

    public class RealEstateRepository: GameDbRepository<RealEstateEntity>, IRealEstateRepository {
        public RealEstateRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<IEnumerable<RealEstateEntity>>> GetByOwnerIdAsync(long ownerId) {
            try {
                var realEstates = await _dbSet
                    .Where(r => r.OwnerId == ownerId)
                    .ToListAsync();
                if (realEstates.Count == 0) {
                    return new DbQueryResult<IEnumerable<RealEstateEntity>>(DbResultType.Warning, "No real estates found.");
                }
                return new DbQueryResult<IEnumerable<RealEstateEntity>>(DbResultType.Success, "Real estates found successfully.", realEstates);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<RealEstateEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<RealEstateEntity>> GetByAddressIdAsync(long addressId) {
            try {
                var realEstate = await _dbSet
                    .FirstOrDefaultAsync(r => r.AddressId == addressId);
                if (realEstate == null) {
                    return new DbQueryResult<RealEstateEntity>(DbResultType.Warning, "Real estate not found.");
                }
                return new DbQueryResult<RealEstateEntity>(DbResultType.Success, "Real estate found successfully.", realEstate);
            } catch (Exception ex) {
                return new DbQueryResult<RealEstateEntity>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}