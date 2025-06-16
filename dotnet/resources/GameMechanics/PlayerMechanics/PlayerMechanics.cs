using System;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Repository;
using GameDb.Service;
using GTANetworkAPI;

namespace GameMechanics.PlayerMechanics
{
    public class PlayerMechanics
    {
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

        public static async Task<bool> SavePlayerData(Player player)
        {
            var result = await GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name);
            if (result.ResultType == DbResultType.Success)
            {
                if (result.ReturnValue == null)
                {
                    Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                    throw new Exception($"Player {player.Name} not found in database.");
                }
                PlayerEntity playerEntity = result.ReturnValue;
                Vector3 position = player.Position;
                var sPosResult = await GameDbContainer.PlayerService.SetPositionAsync(playerEntity, position.X, position.Y, position.Z);
                var sHPResult = await GameDbContainer.PlayerService.SetHPAsync(playerEntity, (byte)player.Health);

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

                return true;
            }

            Console.WriteLine($"Error retrieving player data: {result.Message}");
            return false;
        }

        public static async Task<bool> LoadPlayerData(Player player)
        {
            var result = await GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name);
            if (result.ResultType == DbResultType.Success)
            {
                if (result.ReturnValue == null)
                {
                    Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                    throw new Exception($"Player {player.Name} not found in database.");
                }
                PlayerEntity playerEntity = result.ReturnValue;
                Vector3 position = new Vector3(playerEntity.PositionX, playerEntity.PositionY, playerEntity.PositionZ);
                TeleportPlayer(player, position);
                player.Health = playerEntity.HP;

                return true;
            }

            Console.WriteLine($"Error retrieving player data: {result.Message}");
            return false;
        }

        public static void InitializeStats(Player player)
        {
            player.SetSharedData("Hunger", 100);
            player.SetSharedData("Thirst", 100);
        }

        public static void AdjustStats(Player player, string type)
        {
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : 100;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : 100;

            if (type == "food")
            {
                hunger = Math.Min(100, hunger + 20);
            }
            else if (type == "drink")
            {
                thirst = Math.Min(100, thirst + 20);
            }

            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);

            player.TriggerEvent("Player:UpdateStats", hunger, thirst);
        }

        public static void Consume(Player player, string type)
        {
            AdjustStats(player, type);
            player.SendChatMessage($"~g~You consumed {type}!");
        }

        public static void ReduceStats(Player player, int hungerLoss = 1, int thirstLoss = 1)
        {
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : 100;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : 100;

            hunger = Math.Max(0, hunger - hungerLoss);
            thirst = Math.Max(0, thirst - thirstLoss);

            player.SetSharedData("Hunger", hunger);
            player.SetSharedData("Thirst", thirst);

            player.TriggerEvent("Player:UpdateStats", hunger, thirst);
        }

        public static void CheckSurvival(Player player)
        {
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : 100;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : 100;

            if (hunger == 0 || thirst == 0)
            {
                player.Health = Math.Max(1, player.Health - 5);
                player.SendChatMessage("~r~You're starving or dehydrated!");
            }
        }
    }
}