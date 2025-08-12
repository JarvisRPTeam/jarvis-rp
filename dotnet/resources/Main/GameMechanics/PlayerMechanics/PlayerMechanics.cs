using System;
using System.Collections.Generic;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using GameDb.Service;
using GTANetworkAPI;

namespace Main.GameMechanics.PlayerMechanics
{
    public class PlayerMechanics
    {
        public static readonly HashSet<Player> PlayersOnPasswordEntry = new HashSet<Player>();
        private const int MaxStat = 100;
        private const int StarvationDamage = 5;
        private const int ModerateDamage = 2;
        private const int WarningThreshold = 20;
        private const int CriticalThreshold = 10;

        public static bool SavePlayerData(Player player)
        {
            var playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();

            if (playerEntity == null)
            {
                Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                throw new Exception($"Player {player.Name} not found in database.");
            }

            Vector3 position = player.Position;
            PositionModel playerPosition = new PositionModel
            {
                X = position.X,
                Y = position.Y,
                Z = position.Z,
                Heading = player.Heading
            };

            bool sPosResult = GameDbContainer.PlayerService.SetPositionAsync(playerEntity, playerPosition).GetAwaiter().GetResult();
            bool sHPResult = GameDbContainer.PlayerService.SetHPAsync(playerEntity, (byte)player.Health).GetAwaiter().GetResult();
            bool sHungerResult = GameDbContainer.PlayerService.SetHungerAsync(playerEntity, GetHunger(player)).GetAwaiter().GetResult();
            bool sThirstResult = GameDbContainer.PlayerService.SetThirstAsync(playerEntity, GetThirst(player)).GetAwaiter().GetResult();
            bool sCashResult = GameDbContainer.PlayerService.SetCashAsync(playerEntity, GetMoney(player)).GetAwaiter().GetResult();
            bool sStaminaResult = GameDbContainer.PlayerService.SetStaminaAsync(playerEntity, GetStamina(player)).GetAwaiter().GetResult();
            


            if (!sPosResult)
            {
                Console.WriteLine($"Error setting position!");
                return false;
            }
            if (!sHPResult)
            {
                Console.WriteLine($"Error setting HP!");
                return false;
            }
            if (!sHungerResult)
            {
                Console.WriteLine($"Error setting Hunger!");
                return false;
            }
            if (!sThirstResult)
            {
                Console.WriteLine($"Error setting thirst!");
                return false;
            }
            if (!sCashResult)
            {
                Console.WriteLine($"Error setting money!");
                return false;
            }
            if (!sStaminaResult)
            {
                Console.WriteLine($"Error setting stamina!");
                return false;
            }

            return true;
        }

        public static bool LoadPlayerData(Player player)
        {
            var playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();

            if (playerEntity == null)
            {
                Console.WriteLine($"FATAL: Player not found in database.");
                return false;
            }

            Vector3 position = new Vector3(playerEntity.Position.X, playerEntity.Position.Y, playerEntity.Position.Z);

            TeleportPlayer(player, position, playerEntity.Position.Heading);

            player.Health = playerEntity.HP;

            long money = playerEntity.Cash;
            player.SetSharedData("Money", money);
            UpdateClientMoney(player);

            int hunger = playerEntity.Hunger;
            int thirst = playerEntity.Thirst;
            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);
            player.TriggerEvent("Player:UpdateStats", hunger, thirst);

            return true;
        }

        public static void InitializeStats(Player player)
        {
            Console.WriteLine($"Initializing stats for player {player.Name}");
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            player.SetSharedData("Hunger", 50);
            player.SetSharedData("Thirst", 50);
            player.SetSharedData("Stamina", 100);
            player.SetSharedData("Money", 0L);
            player.SetSharedData("Health", 100);
            player.Health = 100; // Reset health to full
        }

        public static void AdjustStats(Player player, string type, int value)
        {
            int hunger = GetHunger(player);
            int thirst = GetThirst(player);

            if (type == "food")
                hunger = Math.Min(MaxStat, hunger + value);
            else if (type == "drink")
                thirst = Math.Min(MaxStat, thirst + value);

            SetStats(player, hunger, thirst, GetStamina(player));
        }

        public static void SetStats(Player player, int hunger, int thirst, int stamina)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            SetHunger(player, hunger);
            SetThirst(player, thirst);
            SetStamina(player, stamina);
            player.TriggerEvent("Player:UpdateStats", hunger, thirst);
        }

        public static void ResetStats(Player player)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            SetHunger(player, 50);
            SetThirst(player, 50);
            SetStamina(player, 100);
            player.Health = 100; // Reset health to full
            UpdateClientStats(player);
        }

        public static void UpdateClientStats(Player player)
        {
            if (!player.Exists) return;
            player.TriggerEvent("client:updateStats", GetHunger(player), GetThirst(player));
        }

        public static void CheckSurvival(Player player)
        {
            int hunger = GetHunger(player);
            int thirst = GetThirst(player);

            if (hunger <= WarningThreshold || thirst <= WarningThreshold)
                player.SendChatMessage("~y~You're feeling weak...");

            if (hunger <= CriticalThreshold || thirst <= CriticalThreshold)
            {
                player.Health = Math.Max(1, player.Health - ModerateDamage);
                player.SendChatMessage("~o~You're starving or very thirsty!");
            }

            if (hunger == 0 || thirst == 0)
            {
                player.Health = Math.Max(1, player.Health - StarvationDamage);
                player.SendChatMessage("~r~You're dying of hunger or thirst!");
            }
        }

        public static void PlayerStatus(Player player)
        {
            player.SendChatMessage($"~o~Hunger: {GetHunger(player)}");
            player.SendChatMessage($"~b~Thirst: {GetThirst(player)}");
            player.SendChatMessage($"~r~Health: {player.Health}");
        }

        public static void RecoverFullHealth(Player player)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            player.Health = 100;
            player.SendChatMessage("~g~You have fully recovered your health.");
        }

        public static void TeleportPlayer(Player player, Vector3 position, float heading)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            player.Position = position;
            player.Heading = heading;
            Console.WriteLine($"Player {player.Name} position set to {position} and heading set to {heading}.");
        }

        public static void GetPlayerInfo(Player player)
        {
            player.SendChatMessage("~y~Player Info:");
            player.SendChatMessage($"~w~Name: {player.Name}");
            player.SendChatMessage($"~g~Money: ${GetMoney(player)}");
            player.SendChatMessage($"~o~Hunger: {GetHunger(player)}");
            player.SendChatMessage($"~b~Thirst: {GetThirst(player)}");
            player.SendChatMessage($"~g~Stamina: {GetStamina(player)}");
            player.SendChatMessage($"~r~Health: {player.Health}");
        }

        public static long GetMoney(Player player)
        {
            if (player.HasSharedData("Money"))
            {
                return player.GetSharedData<long>("Money");
            }
            throw new Exception($"Player {player.Name} does not have Money data set.");
        }

        public static void AddMoney(Player player, long amount)
        {
            long currentMoney = GetMoney(player);
            long newMoney = Math.Max(0, currentMoney + amount);
            SetMoney(player, newMoney);
        }

        public static void SetMoney(Player player, long amount)
        {
            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            bool setMoneyResult = GameDbContainer.PlayerService.SetCashAsync(playerEntity, amount).GetAwaiter().GetResult();

            if (!setMoneyResult)
            {
                Console.WriteLine($"Error setting money for player {player.Name}");
                return;
            }
            player.SetSharedData("Money", amount);
            UpdateClientMoney(player);
        }

        public static void UpdateClientMoney(Player player)
        {
            long money = GetMoney(player);
            player.TriggerEvent("UpdateMoneyUI", money);
        }

        public static byte GetHunger(Player player)
        {
            if (player.HasSharedData("Hunger"))
            {
                return (byte)player.GetSharedData<int>("Hunger");
            }
            throw new Exception($"Player {player.Name} does not have Hunger data set.");
        }

        public static void SetHunger(Player player, int amount)
        {
            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            bool setHungerResult = GameDbContainer.PlayerService.SetHungerAsync(playerEntity, (byte)amount).GetAwaiter().GetResult();

            if (!setHungerResult)
            {
                Console.WriteLine($"Error setting hunger for player {player.Name}");
                return;
            }
            UpdateClientStats(player);
        }

        public static void DrainHunger(Player player)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            int hunger = GetHunger(player);
            hunger = Math.Max(0, hunger - 1);
            SetHunger(player, (byte)hunger);
            UpdateClientStats(player);
        }

        public static void EatFood(Player player, int foodValue)
        {
            int hunger = GetHunger(player);
            hunger = Math.Min(MaxStat, hunger + foodValue);
            SetHunger(player, hunger);
            UpdateClientStats(player);
            player.SendChatMessage($"~o~{player.Name} ate food and restored {foodValue} hunger points. Current hunger: {hunger}.");
        }

        public static byte GetThirst(Player player)
        {
            if (player.HasSharedData("Thirst"))
            {
                return (byte)player.GetSharedData<int>("Thirst");
            }
            throw new Exception($"Player {player.Name} does not have Thirst data set.");
        }

        public static void SetThirst(Player player, int amount)
        {
            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            bool setThirstResult = GameDbContainer.PlayerService.SetThirstAsync(playerEntity, (byte)amount).GetAwaiter().GetResult();

            if (!setThirstResult)
            {
                Console.WriteLine($"Error setting Thirst data for player {player.Name}");
                return;
            }

            UpdateClientStats(player);
        }

        public static void DrainThirst(Player player)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            int thirst = GetThirst(player);
            thirst = Math.Max(0, thirst - 1);
            SetThirst(player, (byte)thirst);
            UpdateClientStats(player);
        }

        public static void DrinkWater(Player player, int drinkValue)
        {
            int thirst = GetThirst(player);
            thirst = Math.Min(MaxStat, thirst + drinkValue);
            SetThirst(player, (byte)thirst);
            UpdateClientStats(player);
            player.SendChatMessage($"~b~{player.Name} drank water and restored {drinkValue} thirst points. Current thirst: {thirst}.");
        }

        public static byte GetStamina(Player player)
        {
            if (player.HasSharedData("Stamina"))
            {
                return (byte)player.GetSharedData<int>("Stamina");
            }
            throw new Exception($"Player {player.Name} does not have Stamina data set.");
        }

        public static void SetStamina(Player player, int stamina)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            player.SetSharedData("Stamina", stamina);
            player.TriggerEvent("Player:UpdateStamina", stamina);
        }

        public static List<PunishmentDisplayModel> GetActivePunishments(Player player)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                throw new Exception("Player does not exist.");
            }

            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            if (playerEntity == null)
            {
                Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                throw new Exception("Player not found.");
            }

            List<PunishmentDisplayModel> punishments = new List<PunishmentDisplayModel>();

            foreach (var punishment in playerEntity.Punishments)
            {
                if (!punishment.IsCancelled && (punishment.Timeout == null || punishment.Timeout > DateTime.UtcNow))
                {
                    punishments.Add(new PunishmentDisplayModel
                    {
                        Type = punishment.Type,
                        Reason = punishment.Reason,
                        Timeout = punishment.Timeout
                    });
                }
            }

            return punishments;
        }

        public static bool EnsureNotBanned(Player player)
        {
            List<PunishmentDisplayModel> activePunishments = GetActivePunishments(player);
            if (activePunishments.Count > 0)
            {
                string punishmentsMessage = "You have active punishments:\n";
                foreach (var punishment in activePunishments)
                {
                    punishmentsMessage += $"- {punishment.Type}: {punishment.Reason} (Expires: {punishment.Timeout?.ToString("g") ?? "Permanent"})\n";
                }
                player.Kick(punishmentsMessage);
                return false;
            }
            return true;
        }

        public static bool EnsureRegistered(Player player)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                throw new Exception("Player does not exist.");
            }

            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            if (playerEntity == null)
            {
                player.Kick("You are not registered. Please register first.");
                return false;
            }

            return true;
        }
    }
}
