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

        Task<DbQueryResult<ResidenceEntity>> GetByIdAsync(long playerId, long realEstateId);
        Task<DbQueryResult<ResidenceEntity>> DeleteByIdAsync(long playerId, long realEstateId);
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

        public async Task<DbQueryResult<ResidenceEntity>> GetByIdAsync(long playerId, long realEstateId) {
            try {
                var entity = await _dbSet.FindAsync(playerId, realEstateId);
                if (entity == null) {
                    return new DbQueryResult<ResidenceEntity>(DbResultType.Warning, "Residence not found.");
                }
                return new DbQueryResult<ResidenceEntity>(DbResultType.Success, "Residence found.", entity);
            } catch (Exception ex) {
                return new DbQueryResult<ResidenceEntity>(DbResultType.Error, $"Error retrieving residence: {ex.Message}");
            }
        }

        public async Task<DbQueryResult<ResidenceEntity>> DeleteByIdAsync(long playerId, long realEstateId) {
            var searchResult = await GetByIdAsync(playerId, realEstateId);
            if (searchResult.ReturnValue == null) {
                return new DbQueryResult<ResidenceEntity>(DbResultType.Warning, searchResult.Message);
            }
            if (searchResult.ResultType == DbResultType.Error) {
                return searchResult;
            }

            var entity = searchResult.ReturnValue;
            try {
                _dbSet.Remove(entity);
                return new DbQueryResult<ResidenceEntity>(DbResultType.Success, "Residence deleted successfully.");
            } catch (Exception ex) {
                return new DbQueryResult<ResidenceEntity>(DbResultType.Error, $"Error deleting residence: {ex.Message}");
            }
        }

        public override Task<DbQueryResult<ResidenceEntity>> GetByIdAsync(long id) =>
            throw new NotSupportedException("Use GetByIdAsync(long playerId, long realEstateId) for ResidenceEntity.");

        public override Task<DbQueryResult<ResidenceEntity>> DeleteByIdAsync(long id) =>
            throw new NotSupportedException("Use DeleteByIdAsync(long playerId, long realEstateId) for ResidenceEntity.");
    }
}