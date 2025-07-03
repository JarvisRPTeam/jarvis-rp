using GTANetworkAPI;
using System;
using GameDb.Service;
using GameDb.Domain.Models;
namespace GameMechanics.PlayerMechanics
{
    public class PlayerEvents : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            // GameDbContainer.PlayerService.RegisterPlayerAsync(new PlayerCreateModel
            // {
            //     Nickname = "WeirdNewbie",
            //     Password = "123",
            // }).GetAwaiter().GetResult();

            // var result = PlayerMechanics.LoadPlayerData(player);
            // if (!result)
            // {
            //     Console.WriteLine($"Error loading player {player.Name} data");
            // }
            // PlayerMechanics.InitializeStats(player);
        }
        [ServerEvent(Event.PlayerSpawn)]
        public void OnPlayerSpawn(Player player)
        {
            var result = PlayerMechanics.LoadPlayerData(player);

            if (!result)
            {
                Console.WriteLine($"Error loading player {player.Name} data");
            }

            PlayerMechanics.CheckSurvival(player);
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Player player, Player killer, uint reason)
        {
            // Handle player death logic here
            // For example, you might want to reset stats or notify other players
            Console.WriteLine($"Player {player.Name} has died.");

            // Optionally, you can reset stats or perform other actions
            PlayerMechanics.ResetStats(player);
            PlayerMechanics.UpdateClientStats(player);
        }


        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Player player, DisconnectionType type, string reason)
        {
            var result = PlayerMechanics.SavePlayerData(player);
            if (!result)
            {
                Console.WriteLine($"Error saving player {player.Name} data");
            }

            Console.WriteLine($"Player {player.Name} disconnected. Reason: {reason}, Type: {type}");
        }



        // This event fires every frame for each player, so we use it to do periodic stat reduction & checks
        [RemoteEvent("Player:UpdateStats")]
        public void OnPlayerUpdateStats(Player player)
        {
            PlayerMechanics.CheckSurvival(player);
            PlayerMechanics.UpdateClientStats(player);
        }

        [RemoteEvent("Player:DrainHunger")]
        public void OnPlayerDrainHunger(Player player)
        {
            PlayerMechanics.DrainHunger(player);
        }
        
        [RemoteEvent("Player:DrainThirst")]
        public void OnPlayerDrainThirst(Player player)
        {
           PlayerMechanics.DrainThirst(player);
        }

    }
}
