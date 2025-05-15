#nullable enable
using GameDb.Repository;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using System.Threading.Tasks;

namespace GameDb.Service
{
    public interface IPlayerService {
        Task<DbQueryResult<PlayerEntity>> GetPlayerByNicknameAsync(string nickname);
        Task<DbQueryResult<PlayerEntity>> RegisterPlayerAsync(PlayerCreateModel playerModel);
        Task<DbQueryResult<PlayerEntity>> DealCashAsync(PlayerEntity player, long amount);
        Task<DbQueryResult<PlayerEntity>> DealCashAsync(long playerId, long amount);
        Task<DbQueryResult<PlayerEntity>> DealHPAsync(PlayerEntity player, byte amount);
        Task<DbQueryResult<PlayerEntity>> DealHPAsync(long playerId, byte amount);
        Task<DbQueryResult<PlayerEntity>> DealHungerAsync(PlayerEntity player, byte amount);
        Task<DbQueryResult<PlayerEntity>> DealHungerAsync(long playerId, byte amount);
        Task<DbQueryResult<PlayerEntity>> DealThirstAsync(PlayerEntity player, byte amount);
        Task<DbQueryResult<PlayerEntity>> DealThirstAsync(long playerId, byte amount);
        Task<DbQueryResult<PlayerEntity>> DealStaminaAsync(PlayerEntity player, byte amount);
        Task<DbQueryResult<PlayerEntity>> DealStaminaAsync(long playerId, byte amount);
        Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(PlayerEntity player, SocialClubEntity? socialClub);
        Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(long playerId, long socialClubId);
        Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(long playerId, SocialClubEntity? socialClub);
        Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(PlayerEntity player, long socialClubId);
        Task<DbQueryResult<PlayerEntity>> SetPositionAsync(PlayerEntity player, float x, float y, float z);
        Task<DbQueryResult<PlayerEntity>> SetPositionAsync(long playerId, float x, float y, float z);
        Task<DbQueryResult<PlayerEntity>> SetPositionAsync(long playerId, PositionModel position);
        Task<DbQueryResult<PositionModel>> GetPositionAsync(PlayerEntity player);
        Task<DbQueryResult<PositionModel>> GetPositionAsync(long playerId);
    }
    
    public class PlayerService: IPlayerService {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISocialClubRepository _socialClubRepository;
        
        public PlayerService(
            IPlayerRepository playerRepository, 
            ISocialClubRepository socialClubRepository) {
            _socialClubRepository = socialClubRepository;
            _playerRepository = playerRepository;
        }
        
        public async Task<DbQueryResult<PlayerEntity>> GetPlayerByNicknameAsync(string nickname) {
            var result = await _playerRepository.GetByNicknameAsync(nickname);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }
            return result;
        }

        public async Task<DbQueryResult<PlayerEntity>> RegisterPlayerAsync(PlayerCreateModel playerModel)
        {
            var playerEntity = new PlayerEntity
            {
                // Id is not set, assuming auto-increment by DB
                Nickname = playerModel.Nickname,
                Password = playerModel.Password,
                Cash = 2000,
                HP = 100,
                Hunger = 100,
                Thirst = 100,
                Stamina = 100,
                SocialClubId = null,
                PositionX = 0,
                PositionY = 0,
                PositionZ = 0
            };

            var addResult = await _playerRepository.AddAsync(playerEntity);
            if (addResult.ResultType != DbResultType.Success)
            {
                return addResult;
            }

            var saved = await _playerRepository.SaveChangesAsync();
            if (!saved)
            {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to save player to database.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player created successfully.", playerEntity);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealCashAsync(PlayerEntity player, long amount) {
            if (player == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }

            player.Cash += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to update player cash.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player cash updated successfully.", player);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealCashAsync(long playerId, long amount) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            return await DealCashAsync(player, amount);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealHPAsync(PlayerEntity player, byte amount) {
            if (player == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }

            player.HP += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to update player HP.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player HP updated successfully.", player);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealHPAsync(long playerId, byte amount) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            return await DealHPAsync(player, amount);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealHungerAsync(PlayerEntity player, byte amount) {
            if (player == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }

            player.Hunger += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to update player hunger.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player hunger updated successfully.", player);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealHungerAsync(long playerId, byte amount) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            return await DealHungerAsync(player, amount);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealThirstAsync(PlayerEntity player, byte amount) {
            if (player == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }

            player.Thirst += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to update player thirst.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player thirst updated successfully.", player);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealThirstAsync(long playerId, byte amount) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            return await DealThirstAsync(player, amount);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealStaminaAsync(PlayerEntity player, byte amount) {
            if (player == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }

            player.Stamina += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to update player stamina.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player stamina updated successfully.", player);
        }

        public async Task<DbQueryResult<PlayerEntity>> DealStaminaAsync(long playerId, byte amount) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            return await DealStaminaAsync(player, amount);
        }
        
        public async Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(PlayerEntity player, SocialClubEntity? socialClub) {
            if (player == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }

            player.SocialClub = socialClub;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to assign social club to player.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Social club assigned successfully.", player);
        }

        public async Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(long playerId, long socialClubId) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            var socialClubResult = await _socialClubRepository.GetByIdAsync(socialClubId);
            if (socialClubResult.ResultType != DbResultType.Success || socialClubResult.ReturnValue == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Social club not found.");
            }

            var socialClub = socialClubResult.ReturnValue;
            return await AssignSocialClubAsync(player, socialClub);
        }

        public async Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(long playerId, SocialClubEntity? socialClub) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            return await AssignSocialClubAsync(player, socialClub);
        }

        public async Task<DbQueryResult<PlayerEntity>> AssignSocialClubAsync(PlayerEntity player, long socialClubId) {
            var socialClubResult = await _socialClubRepository.GetByIdAsync(socialClubId);
            if (socialClubResult.ResultType != DbResultType.Success || socialClubResult.ReturnValue == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Social club not found.");
            }

            var socialClub = socialClubResult.ReturnValue;
            return await AssignSocialClubAsync(player, socialClub);
        }

        public async Task<DbQueryResult<PlayerEntity>> SetPositionAsync(PlayerEntity player, float x, float y, float z) {
            if (player == null) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Player not found.");
            }

            player.PositionX = x;
            player.PositionY = y;
            player.PositionZ = z;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult) {
                return new DbQueryResult<PlayerEntity>(DbResultType.Error, "Failed to update player position.");
            }

            return new DbQueryResult<PlayerEntity>(DbResultType.Success, "Player position updated successfully.", player);
        }

        public async Task<DbQueryResult<PlayerEntity>> SetPositionAsync(long playerId, float x, float y, float z) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return searchResult;
            }

            var player = searchResult.ReturnValue;
            return await SetPositionAsync(player, x, y, z);
        }

        public async Task<DbQueryResult<PlayerEntity>> SetPositionAsync(long playerId, PositionModel position) {
            return await SetPositionAsync(playerId, position.X, position.Y, position.Z);
        }

        public Task<DbQueryResult<PositionModel>> GetPositionAsync(PlayerEntity player) {
            if (player == null) {
                return Task.FromResult(new DbQueryResult<PositionModel>(DbResultType.Error, "Player not found."));
            }

            var positionModel = new PositionModel {
                X = player.PositionX,
                Y = player.PositionY,
                Z = player.PositionZ
            };

            return Task.FromResult(new DbQueryResult<PositionModel>(DbResultType.Success, "Player position retrieved successfully.", positionModel));
        }

        public async Task<DbQueryResult<PositionModel>> GetPositionAsync(long playerId) {
            var searchResult = await _playerRepository.GetByIdAsync(playerId);
            if (searchResult.ResultType != DbResultType.Success || searchResult.ReturnValue == null) {
                return new DbQueryResult<PositionModel>(DbResultType.Error, "Player not found.");
            }

            var player = searchResult.ReturnValue;
            return await GetPositionAsync(player);
        }
    }
}
