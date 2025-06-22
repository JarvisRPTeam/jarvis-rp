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
        Task<RoleEntity?> GetRoleByNameAsync(string roleName);
        Task<bool> GrantRoleToPlayerAsync(PlayerEntity player, RoleEntity role);
        Task<PunishmentEntity?> CreatePunishmentAsync(PunishmentCreateModel punishmentModel);
        Task<bool> RemovePunishmentAsync(PunishmentEntity punishment);
        Task<bool> ClearAllPunishmentsAsync(PlayerEntity player);
    }

    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly ISocialClubRepository _socialClubRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPunishmentRepository _punishmentRepository;
        private readonly GameDbContext _context;

        public PlayerService(
            IPlayerRepository playerRepository,
            ISocialClubRepository socialClubRepository,
            IInventoryRepository inventoryRepository,
            IRoleRepository roleRepository,
            IPunishmentRepository punishmentRepository,
            GameDbContext context
        )
        {
            _socialClubRepository = socialClubRepository;
            _playerRepository = playerRepository;
            _inventoryRepository = inventoryRepository;
            _roleRepository = roleRepository;
            _punishmentRepository = punishmentRepository;
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
                RoleEntity? role = await GetRoleByNameAsync("Player");
                if (role == null)
                {
                    Console.WriteLine("Role 'Player' not found.");
                    return null;
                }
                var playerEntity = new PlayerEntity
                {
                    Nickname = playerModel.Nickname,
                    Password = playerModel.Password,
                    Cash = 2000,
                    HP = 100,
                    Hunger = 100,
                    Thirst = 100,
                    Stamina = 100,
                    Breath = 100,
                    Strength = 0,
                    Endurance = 0,
                    Stealth = 0,
                    DrivingSkill = 0,
                    ShootingSkill = 0,
                    FishingSkill = 0,
                    HuntingSkill = 0,
                    FlyingSkill = 0,
                    BreathHoldingSkill = 0,
                    SocialClubId = null,
                    Position = new PositionModel
                    {
                        X = 0,
                        Y = 0,
                        Z = 0,
                        Heading = 0
                    },
                    RoleId = role.Id,
                    PlayedToday = TimeSpan.Zero,
                    PlayedTotal = TimeSpan.Zero,
                    BankBalance = null,
                    BankCardNumber = null,
                    BankCardPIN = null,
                    SpawnPlaceId = null,
                    JarvisBalance = 0,
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

            if (position == null)
            {
                Console.WriteLine("Position cannot be null.");
                return false;
            }

            player.Position = position;

            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to update player position.");
                return false;
            }

            return true;
        }

        public async Task<RoleEntity?> GetRoleByNameAsync(string roleName)
        {
            var result = await _roleRepository.GetByNameAsync(roleName);
            if (result.ResultType != DbResultType.Success || result.ReturnValue == null)
            {
                Console.WriteLine($"Error retrieving role by name '{roleName}': {result.Message}");
                return null;
            }
            return result.ReturnValue;
        }

        public async Task<bool> GrantRoleToPlayerAsync(PlayerEntity player, RoleEntity role)
        {
            if (player == null)
            {
                Console.WriteLine("Player cannot be null.");
                return false;
            }

            if (role == null)
            {
                Console.WriteLine("Role cannot be null.");
                return false;
            }

            player.RoleId = role.Id;

            var updateResult = await _playerRepository.SaveChangesAsync();
            if (!updateResult)
            {
                Console.WriteLine("Failed to grant role to player.");
                return false;
            }

            return true;
        }

        public async Task<PunishmentEntity?> CreatePunishmentAsync(PunishmentCreateModel punishmentModel)
        {
            if (punishmentModel == null)
            {
                Console.WriteLine("Punishment model cannot be null.");
                return null;
            }

            if (punishmentModel.PlayerId <= 0)
            {
                Console.WriteLine("Invalid player ID.");
                return null;
            }

            var timeOut = DateTime.UtcNow.AddSeconds(punishmentModel.Duration.TotalSeconds);
            var punishmentEntity = new PunishmentEntity
            {
                PlayerId = punishmentModel.PlayerId,
                Type = punishmentModel.Type,
                Reason = punishmentModel.Reason,
                Timeout = timeOut,
                AdminId = punishmentModel.AdminId
            };

            var addResult = await _punishmentRepository.AddAsync(punishmentEntity);
            if (addResult.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error creating punishment: {addResult.Message}");
                return null;
            }
            var saved = await _punishmentRepository.SaveChangesAsync();
            if (!saved)
            {
                Console.WriteLine("Failed to save punishment to database.");
                return null;
            }
            return punishmentEntity;
        }

        public async Task<bool> RemovePunishmentAsync(PunishmentEntity punishment)
        {
            if (punishment == null)
            {
                Console.WriteLine("Punishment cannot be null.");
                return false;
            }

            var deleteResult = await _punishmentRepository.DeleteByIdAsync(punishment.Id);
            if (deleteResult.ResultType != DbResultType.Success)
            {
                Console.WriteLine($"Error removing punishment: {deleteResult.Message}");
                return false;
            }

            var saved = await _punishmentRepository.SaveChangesAsync();
            if (!saved)
            {
                Console.WriteLine("Failed to save changes after removing punishment.");
                return false;
            }

            return true;
        }

        public async Task<bool> ClearAllPunishmentsAsync(PlayerEntity player)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (player == null)
                {
                    Console.WriteLine("Player cannot be null.");
                    return false;
                }

                var punishments = player.Punishments;
                if (punishments == null || punishments.Count == 0)
                {
                    Console.WriteLine("No punishments to clear for player.");
                    await transaction.CommitAsync();
                    return true;
                }

                foreach (var punishment in punishments)
                {
                    var deleteResult = await _punishmentRepository.DeleteByIdAsync(punishment.Id);
                    if (deleteResult.ResultType != DbResultType.Success)
                    {
                        Console.WriteLine($"Error removing punishment with ID {punishment.Id}: {deleteResult.Message}");
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during clearing punishments: {ex.Message}");
                await transaction.RollbackAsync();
                return false;
            }
        }            
    }
}
