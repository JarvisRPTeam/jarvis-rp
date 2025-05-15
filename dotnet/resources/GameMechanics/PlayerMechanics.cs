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
            NAPI.Chat.SendChatMessageToPlayer(player, $"Your position has been set to {position}.");
        }
    }
}