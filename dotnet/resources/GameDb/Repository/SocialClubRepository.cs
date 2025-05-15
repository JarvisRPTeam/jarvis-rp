using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository {
    public interface ISocialClubRepository: IGameDbRepository<SocialClubEntity> {
        Task<DbQueryResult<IEnumerable<SocialClubEntity>>> GetByNameAsync(string name);
    }

    public class SocialClubRepository: GameDbRepository<SocialClubEntity>, ISocialClubRepository {
        public SocialClubRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<IEnumerable<SocialClubEntity>>> GetByNameAsync(string name) {
            try {
                var clubs = await _dbSet
                    .Where(s => s.Name == name)
                    .ToListAsync();
                if (clubs.Count == 0) {
                    return new DbQueryResult<IEnumerable<SocialClubEntity>>(DbResultType.Warning, "No social clubs found.");
                }
                return new DbQueryResult<IEnumerable<SocialClubEntity>>(DbResultType.Success, "Social clubs found successfully.", clubs);
            } catch (Exception ex) {
                return new DbQueryResult<IEnumerable<SocialClubEntity>>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}