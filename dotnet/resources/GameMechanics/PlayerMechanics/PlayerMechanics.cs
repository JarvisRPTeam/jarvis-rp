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
    }
}