using System.Threading.Tasks;
using GameDb.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace GameDb.Repository {
    public interface IPlayerRepository: IGameDbRepository<PlayerEntity> {
        Task<DbQueryResult<PlayerEntity>> GetByNicknameAsync(string nickname);
    }

    public class PlayerRepository: GameDbRepository<PlayerEntity>, IPlayerRepository {

        public PlayerRepository(GameDbContext context) : base(context) {
        }

        public async Task<DbQueryResult<PlayerEntity>> GetByNicknameAsync(string nickname) {
            try {
                PlayerEntity player = await _dbSet
                    .FirstOrDefaultAsync(p => p.Nickname == nickname);
                if (player == null) {
                    return new DbQueryResult<PlayerEntity>(DbResultType.Warning, "Player not found.");
                }
                return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player found successfully.", player);
            } catch (Exception ex) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, $"Exception: {ex.Message}");
            }
        }
    }
}