using System;
using GTANetworkAPI;

namespace GameMechanics
{
    public class PlayerMechanics : Script
    {
        public void SetPlayerPosition(Player player, Vector3 position)
        {
            if (player == null || !player.Exists)
            {
                Console.WriteLine("Player does not exist.");
                return;
            }

            player.Position = position;
            Console.WriteLine($"Player {player.Name} position set to {position}.");
        }
    }
}