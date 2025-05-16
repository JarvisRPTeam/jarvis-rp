using System;
using System.Threading.Tasks;
using GameDb.Domain.Entities;
using GameDb.Repository;
using GameDb.Service;
using GTANetworkAPI;

namespace GameMechanics.PlayerMechanics
{
    public class PlayerManager : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public async Task OnPlayerConnected(Player player)
        {
            var result = await PlayerMechanics.LoadPlayerData(player);
            if (!result)
            {
                Console.WriteLine($"Error loading player {player.Name} data");
            }
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public async Task OnPlayerDisconnected(Player player, string reason)
        {
            var result = await PlayerMechanics.SavePlayerData(player);
            if (!result)
            {
                Console.WriteLine($"Error saving player {player.Name} data");
            }
        }
    }
}