#nullable enable
using GameDb.Repository;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using System.Threading.Tasks;
using System;

namespace GameDb.Service
{
    public interface IPlayerService
    {
        Task<PlayerEntity?> GetPlayerByNicknameAsync(string nickname);
        Task<PlayerEntity?> RegisterPlayerAsync(PlayerCreateModel playerModel);
        Task<bool> DealCashAsync(PlayerEntity player, long amount);
        Task<bool> DealHPAsync(PlayerEntity player, byte amount);
        Task<bool> DealHungerAsync(PlayerEntity player, byte amount);
        Task<bool> DealThirstAsync(PlayerEntity player, byte amount);
        Task<bool> DealStaminaAsync(PlayerEntity player, byte amount);
        Task<bool> AssignSocialClubAsync(PlayerEntity player, SocialClubEntity? socialClub);
        Task<bool> SetPositionAsync(PlayerEntity player, PositionModel position);
    }

    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISocialClubRepository _socialClubRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly GameDbContext _context;

        public PlayerService(
            IPlayerRepository playerRepository,
            ISocialClubRepository socialClubRepository,
            IInventoryRepository inventoryRepository,
            GameDbContext context
        )
        {
            _socialClubRepository = socialClubRepository;
            _playerRepository = playerRepository;
            _inventoryRepository = inventoryRepository;
            _context = context;
        }

        public async Task<PlayerEntity?> GetPlayerByNicknameAsync(string nickname)
        {
            var result = await _playerRepository.GetByNicknameAsync(nickname);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving player by nickname '{nickname}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<PlayerEntity?> RegisterPlayerAsync(PlayerCreateModel playerModel)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var playerEntity = new PlayerEntity
                {
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
                    Console.WriteLine($"Error adding player: {addResult.Message}");
                    await transaction.RollbackAsync();
                    return null;
                }

                var inventory = await GameDbContainer.InventoryService.CreateInventoryAsync(playerEntity);

                if (inventory == null)
                {
                    Console.WriteLine("Failed to create inventory for player.");
                    await transaction.RollbackAsync();
                    return null;
                }

                playerEntity.Inventory = inventory;

                var saved = await _playerRepository.SaveChangesAsync();
                if (!saved)
                {
                    Console.WriteLine("Failed to save player to database.");
                    await transaction.RollbackAsync();
                    return null;
                }

                await transaction.CommitAsync();
                return playerEntity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during player registration: {ex.Message}");
                await transaction.RollbackAsync();
                return null;
            }
        }

        public async Task<bool> DealCashAsync(PlayerEntity player, long amount)
        {
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            player.Cash += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to update player cash.");
                return false;
            }

            return true;
        }

        public async Task<bool> DealHPAsync(PlayerEntity player, byte amount)
        {
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            player.HP += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to update player HP.");
                return false;
            }

            return true;
        }

        public async Task<bool> DealHungerAsync(PlayerEntity player, byte amount)
        {
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            player.Hunger += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to update player hunger.");
                return false;
            }

            return true;
        }
        public async Task<bool> DealThirstAsync(PlayerEntity player, byte amount)
        {
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            player.Thirst += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to update player thirst.");
                return false;
            }

            return true;
        }

        public async Task<bool> DealStaminaAsync(PlayerEntity player, byte amount)
        {
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            player.Stamina += amount;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to update player stamina.");
                return false;
            }

            return true;
        }

        public async Task<bool> AssignSocialClubAsync(PlayerEntity player, SocialClubEntity? socialClub)
        {
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            player.SocialClub = socialClub;
            player.SocialClubId = socialClub?.Id;
            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to assign social club to player.");
                return false;
            }

            return true;
        }

        public async Task<bool> SetPositionAsync(PlayerEntity player, PositionModel position)
        {
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return false;
            }

            player.PositionX = position.X;
            player.PositionY = position.Y;
            player.PositionZ = position.Z;
            player.Heading = position.Heading;

            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to update player position.");
                return false;
            }

            return true;
        }
    }
}
