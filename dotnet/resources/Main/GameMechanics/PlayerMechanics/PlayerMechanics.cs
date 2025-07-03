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

            int hunger = playerEntity.Hunger;
            int thirst = playerEntity.Thirst;
            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);
            player.TriggerEvent("Player:UpdateStats", hunger, thirst);

            return true;
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

        public static void ResetStats(Player player)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            SetHunger(player, 50);
            SetThirst(player, 50);
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

        public static void GetPlayerInfo(Player player)
        {
            player.SendChatMessage("~y~Player Info:");
            player.SendChatMessage($"~w~Name: {player.Name}");
            player.SendChatMessage($"~g~Money: ${GetMoney(player)}");
            player.SendChatMessage($"~o~Hunger: {GetHunger(player)}");
            player.SendChatMessage($"~b~Thirst: {GetThirst(player)}");
            player.SendChatMessage($"~r~Health: {player.Health}");
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
            long newMoney = Math.Max(0, currentMoney - amount);
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
            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            int hunger = playerEntity.Hunger;
            return hunger;
        }

        public static void SetHunger(Player player, byte amount)
        {
            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            bool setHungerResult = GameDbContainer.PlayerService.SetHungerAsync(playerEntity, amount).GetAwaiter().GetResult();

            if (!setHungerResult)
            {
                Console.WriteLine($"Error setting money for player {player.Name}");
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
            SetHunger(player, (byte)hunger);
            UpdateClientStats(player);
            player.SendChatMessage($"~o~{player.Name} ate food and restored {foodValue} hunger points. Current hunger: {hunger}.");
        }




        public static void SetThirst(Player player, byte amount)
        {
            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            bool setThirstResult = GameDbContainer.PlayerService.SetThirstAsync(playerEntity, amount).GetAwaiter().GetResult();

            if (!setThirstResult)
            {
                Console.WriteLine($"Error setting Thirst data for player {player.Name}");
                return;
            }

            UpdateClientStats(player);
        }

        public static int GetThirst(Player player)
        {
            PlayerEntity playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            int thirst = playerEntity.Thirst;
            return thirst;
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
    }
}
