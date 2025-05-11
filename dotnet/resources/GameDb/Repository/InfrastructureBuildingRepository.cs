using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository {
    public interface IInfrastructureBuildingRepository: IGameDbRepository<InfrastructureBuildingEntity> {
        Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetBySocialClubIdAsync(long socialClubId);
        Task<DbQueryResult<InfrastructureBuildingEntity>> GetByAddressIdAsync(long addressId);
        Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetByNameAsync(string name);
    }

    public class InfrastructureBuildingRepository: GameDbRepository<InfrastructureBuildingEntity>, IInfrastructureBuildingRepository {
        public InfrastructureBuildingRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetBySocialClubIdAsync(long socialClubId) {
            try {
                var buildings = await _dbSet
                    .Where(b => b.SocialClubId == socialClubId)
                    .ToListAsync();
                if (buildings.Count == 0) {
                    return new DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>(DbResultType.Warning, "No buildings found.");
                }
                return new DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>(DbResultType.Success, "Buildings found successfully.", buildings);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<InfrastructureBuildingEntity>> GetByAddressIdAsync(long addressId) {
            try {
                var building = await _dbSet
                    .FirstOrDefaultAsync(b => b.AddressId == addressId);
                if (building == null) {
                    return new DbQueryResult<InfrastructureBuildingEntity>(DbResultType.Warning, "Building not found.");
                }
                return new DbQueryResult<InfrastructureBuildingEntity>(DbResultType.Success, "Building found successfully.", building);
            } catch (Exception ex) {
                return new DbQueryResult<InfrastructureBuildingEntity>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>> GetByNameAsync(string name) {
            try {
                var buildings = await _dbSet
                    .Where(b => b.Name == name)
                    .ToListAsync();
                if (buildings.Count == 0) {
                    return new DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>(DbResultType.Warning, "No buildings found.");
                }
                return new DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>(DbResultType.Success, "Buildings found successfully.", buildings);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<InfrastructureBuildingEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}