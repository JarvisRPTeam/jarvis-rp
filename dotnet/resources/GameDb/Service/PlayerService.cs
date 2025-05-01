using GameDb.Repository;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using System.Threading.Tasks;

namespace GameDb.Service
{
    public class PlayerService {
        private readonly IGameDbRepository<PlayerEntity> _playerRepository;
        
        public PlayerService(IGameDbRepository<PlayerEntity> playerRepository) {
            _playerRepository = playerRepository;
        }

        public async Task<DbQueryResult<PlayerEntity>> CreatePlayerAsync(PlayerCreateModel playerModel) {
            var playerEntity = new PlayerEntity {
                // Id is not set, assuming auto-increment by DB
                Nickname = playerModel.Nickname,
                Password = playerModel.Password,
                Cash = 0,
                HP = 100,
                Hunger = 100,
                Thirst = 100
            };

            var addResult = await _playerRepository.AddAsync(playerEntity);
            if (addResult.ResultType != DbResultType.Success) {
                return addResult;
            }

            var saved = await _playerRepository.SaveChangesAsync();
            if (!saved) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to save player to database.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player created successfully.", playerEntity);
        }
    }
}
