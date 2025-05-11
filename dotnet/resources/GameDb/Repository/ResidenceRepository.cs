using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameDb.Repository {
    public interface IResidenceRepository: IGameDbRepository<ResidenceEntity> {
        Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetByPlayerIdAsync(long playerId);
        Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetByRealEstateIdAsync(long realEstateId);
    }

    public class ResidenceRepository: GameDbRepository<ResidenceEntity>, IResidenceRepository {
        public ResidenceRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetByPlayerIdAsync(long playerId) {
            try {
                var residences = await _dbSet
                    .Where(r => r.PlayerId == playerId)
                    .ToListAsync();
                if (residences.Count == 0) {
                    return new DbQueryResult<IEnumerable<ResidenceEntity>>(DbResultType.Warning, "No residences found.");
                }
                return new DbQueryResult<IEnumerable<ResidenceEntity>>(DbResultType.Success, "Residences found successfully.", residences);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<ResidenceEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<IEnumerable<ResidenceEntity>>> GetByRealEstateIdAsync(long realEstateId) {
            try {
                var residences = await _dbSet
                    .Where(r => r.RealEstateId == realEstateId)
                    .ToListAsync();
                if (residences.Count == 0) {
                    return new DbQueryResult<IEnumerable<ResidenceEntity>>(DbResultType.Warning, "No residences found.");
                }
                return new DbQueryResult<IEnumerable<ResidenceEntity>>(DbResultType.Success, "Residences found successfully.", residences);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<ResidenceEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}