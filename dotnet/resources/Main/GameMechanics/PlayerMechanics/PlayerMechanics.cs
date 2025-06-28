using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Domain.Models;
using GameDb.Repository;
using GameDb.Service;
using GTANetworkAPI;

namespace GameMechanics.PlayerMechanics
{
    public class PlayerMechanics
    {
        private const int MaxStat = 100;
        private const int HungerIncreaseFood = 20;
        private const int ThirstIncreaseDrink = 20;
        private const int StarvationDamage = 5;
        private const int ModerateDamage = 2;
        private const int WarningThreshold = 20;
        private const int CriticalThreshold = 10;
        private const int ConsumeCooldownSeconds = 5;

        private static Dictionary<Player, DateTime> LastConsumeTime = new Dictionary<Player, DateTime>();


        public static void TeleportPlayer(Player player, Vector3 position)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            player.Position = position;
            Console.WriteLine($"Player {player.Name} position set to {position}.");
        }
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
            bool sHungerResult = GameDbContainer.PlayerService.SetHungerAsync(playerEntity, (byte)GetHunger(player)).GetAwaiter().GetResult();
            bool sThirstResult = GameDbContainer.PlayerService.SetThirstAsync(playerEntity, (byte)GetThirst(player)).GetAwaiter().GetResult();
            bool sMoneyResult = GameDbContainer.PlayerService.SetCashAsync(playerEntity, GetMoney(player)).GetAwaiter().GetResult();


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
            if (!sMoneyResult)
            {
                Console.WriteLine($"Error setting money!");
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

            TeleportPlayer(player, position);

            player.Health = playerEntity.HP;

            long money = playerEntity.Cash;
            player.SetSharedData("Money", money);
            UpdateClientMoney(player);
            

            int hunger = GetHunger(player);
            int thirst = GetThirst(player);
            
            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);
            player.TriggerEvent("Player:UpdateStats", hunger, thirst);

            return true;
        }
        public static void InitializeStats(Player player)
        {
            player.SetSharedData("Hunger", MaxStat);
            player.SetSharedData("Thirst", MaxStat);
            player.TriggerEvent("Player:UpdateStats", MaxStat, MaxStat);
        }

        public static void AdjustStats(Player player, string type, int value)
        {
            int hunger = GetHunger(player);
            int thirst = GetThirst(player);

            if (type == "food")
                hunger = Math.Min(MaxStat, hunger + value);
            else if (type == "drink")
                thirst = Math.Min(MaxStat, thirst + value);

            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);
            player.TriggerEvent("Player:UpdateStats", hunger, thirst);
        }

        public static void Consume(Player player, string type)
        {
            if (LastConsumeTime.TryGetValue(player, out var lastTime) && DateTime.Now < lastTime.AddSeconds(ConsumeCooldownSeconds))
            {
                player.SendChatMessage("~r~You must wait before consuming again.");
                return;
            }

            if (type == "food")
                AdjustStats(player, type, HungerIncreaseFood);
            else if (type == "drink")
                AdjustStats(player, type, ThirstIncreaseDrink);
            else
            {
                player.SendChatMessage("~r~Unknown item.");
                return;
            }

            LastConsumeTime[player] = DateTime.Now;
            player.SendChatMessage($"~g~You consumed {type}!");
        }

        public static void ReduceStats(Player player, int hungerLoss = 1, int thirstLoss = 1)
        {
            int hunger = GetHunger(player);
            int thirst = GetThirst(player);

            hunger = Math.Max(0, hunger - hungerLoss);
            thirst = Math.Max(0, thirst - thirstLoss);

            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);
            player.TriggerEvent("Player:UpdateStats", hunger, thirst);
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

        public static void UpdateClientStats(Player player)
        {
            if (!player.Exists) return;
            player.TriggerEvent("client:updateStats", GetHunger(player), GetThirst(player)); // You must handle this event on the client-side
        }


        public static void AddMoney(Player player, long amount)
        {
            long currentMoney = GetMoney(player);
            long newMoney = currentMoney + amount;
            SetMoney(player, newMoney);
        }
        public static void RemoveMoney(Player player, long amount)
        {
            long currentMoney = GetMoney(player);
            long newMoney = Math.Max(0, currentMoney - amount); // prevent negative
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

        public static long GetMoney(Player player)
        {
            if (player.HasSharedData("Money"))
            {
                return player.GetSharedData<long>("Money");
            }
            throw new Exception($"Player {player.Name} does not have Money data set.");
        }

        public static void UpdateClientMoney(Player player)
        {
            long money = GetMoney(player);
            player.TriggerEvent("UpdateMoneyUI", money); // Klientā jāsaņem long/int atkarībā no implementācijas
        }

        public static int GetHunger(Player player)
        {
            if (player.HasSharedData("Hunger"))
            {
                return player.GetSharedData<int>("Hunger");
            }
            return MaxStat; // Default value if not set
        }

        public static int GetThirst(Player player)
        {
            if (player.HasSharedData("Thirst"))
            {
                return player.GetSharedData<int>("Thirst");
            }
            return MaxStat; // Default value if not set
        }
    }
}
