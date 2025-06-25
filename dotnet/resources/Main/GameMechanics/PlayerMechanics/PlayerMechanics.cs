using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
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
            var result = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            if (result.ResultType == DbResultType.Success)
            {
                if (result.ReturnValue == null)
                {
                    Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                    throw new Exception($"Player {player.Name} not found in database.");
                }
                PlayerEntity playerEntity = result.ReturnValue;
                Vector3 position = player.Position;
                int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : MaxStat;
                int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : MaxStat;

                var sPosResult = GameDbContainer.PlayerService.SetPositionAsync(playerEntity, position.X, position.Y, position.Z).GetAwaiter().GetResult();
                var sHPResult = GameDbContainer.PlayerService.SetHPAsync(playerEntity, (byte)player.Health).GetAwaiter().GetResult();
                var sHungerResult = GameDbContainer.PlayerService.DealHungerAsync(playerEntity.Id, (byte)hunger).GetAwaiter().GetResult();
                var sThirstResult = GameDbContainer.PlayerService.DealThirstAsync(playerEntity.Id, (byte)thirst).GetAwaiter().GetResult();
                var sMoneyResult = GameDbContainer.PlayerService.DealCashAsync(playerEntity.Id, (long)GetMoney(player)).GetAwaiter().GetResult();


                if (sPosResult.ResultType == DbResultType.Error)
                {
                    Console.WriteLine($"Error setting position: {sPosResult.Message}");
                    return false;
                }

                if (sHPResult.ResultType == DbResultType.Error)
                {
                    Console.WriteLine($"Error setting HP: {sHPResult.Message}");
                    return false;
                }

                if (sHungerResult.ResultType == DbResultType.Error)
                {
                    Console.WriteLine($"Error setting hunger: {sHungerResult.Message}");
                    return false;
                }

                if (sThirstResult.ResultType == DbResultType.Error)
                {
                    Console.WriteLine($"Error setting thirst: {sThirstResult.Message}");
                    return false;
                }
                if (sMoneyResult.ResultType == DbResultType.Error)
                {
                    Console.WriteLine($"Error setting money: {sMoneyResult.Message}");
                    return false;
                }
                

                return true;
            }

            Console.WriteLine($"Error retrieving player data: {result.Message}");
            return false;
        }
        public static bool LoadPlayerData(Player player)
        {
            var result = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            if (result.ResultType != DbResultType.Error)
            {
                if (result.ReturnValue == null)
                {
                    Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                    return false;
                }

                PlayerEntity playerEntity = result.ReturnValue;

                Vector3 position = new Vector3(playerEntity.PositionX, playerEntity.PositionY, playerEntity.PositionZ);
                TeleportPlayer(player, position);

                player.Health = playerEntity.HP;

                // ✅ Load & set money
                long money = playerEntity.Cash;
                SetMoney(player, money); // uses sharedData["Money"] and triggers client update

                // ✅ Load hunger and thirst
                int hunger = playerEntity.Hunger > 0 ? playerEntity.Hunger : MaxStat;
                int thirst = playerEntity.Thirst > 0 ? playerEntity.Thirst : MaxStat;
                player.SetSharedData("Hunger", hunger);
                player.SetSharedData("Thirst", thirst);
                player.TriggerEvent("Player:UpdateStats", hunger, thirst);

                return true;
            }

            Console.WriteLine($"Error retrieving player data: {result.Message}");
            return false;
        }




        public static void InitializeStats(Player player)
        {
            player.SetSharedData("Hunger", MaxStat);
            player.SetSharedData("Thirst", MaxStat);
            player.TriggerEvent("Player:UpdateStats", MaxStat, MaxStat);
        }

        public static void AdjustStats(Player player, string type, int value)
        {
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : MaxStat;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : MaxStat;

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
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : MaxStat;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : MaxStat;

            hunger = Math.Max(0, hunger - hungerLoss);
            thirst = Math.Max(0, thirst - thirstLoss);

            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);
            player.TriggerEvent("Player:UpdateStats", hunger, thirst);
        }

        public static void CheckSurvival(Player player)
        {
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : MaxStat;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : MaxStat;

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

            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : MaxStat;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : MaxStat;

            player.TriggerEvent("client:updateStats", hunger, thirst); // You must handle this event on the client-side
        }

        private const string MoneyKey = "PlayerMoney";

        public static void InitializeMoney(Player player)
        {
            if (!player.HasSharedData(MoneyKey))
            {
                player.SetSharedData(MoneyKey, 2000);
                UpdateClientMoney(player);
            }
        }

        public static void AddMoney(Player player, long amount)
        {
            long currentMoney = GetMoney(player);
            long newMoney = currentMoney + amount; // ✅ Proper addition
            SetMoney(player, newMoney);
        }



        public static void SetMoney(Player player, long amount)
        {
            player.SetSharedData("Money", amount);
            UpdateClientMoney(player);
        }


        public static long GetMoney(Player player)
        {
            return player.HasSharedData("Money") ? player.GetSharedData<long>("Money") : 0;
        }



        public static void UpdateClientMoney(Player player)
        {
            long money = GetMoney(player);
            player.TriggerEvent("UpdateMoneyUI", money); // Klientā jāsaņem long/int atkarībā no implementācijas
        }

        public static void RemoveMoney(Player player, long amount)
        {
            long currentMoney = GetMoney(player);
            long newMoney = Math.Max(0, currentMoney - amount); // prevent negative
            SetMoney(player, newMoney);
        }

    }
}
