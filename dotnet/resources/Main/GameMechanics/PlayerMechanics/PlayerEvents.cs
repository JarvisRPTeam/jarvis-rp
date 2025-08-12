using GTANetworkAPI;
using System;
using GameDb.Service;
using GameDb.Domain.Models;
namespace Main.GameMechanics.PlayerMechanics
{
    public class PlayerEvents : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            PlayerMechanics.InitializeStats(player);
            Console.WriteLine($"Stats initialized for player {player.Name}");
            NAPI.Entity.SetEntityTransparency(player, 0);
            Console.WriteLine($"Setting player {player.Name} transparency to 0");
            NAPI.Entity.SetEntityDimension(player, 1);
            Console.WriteLine($"Setting player {player.Name} dimension to 1");
            var registered = PlayerMechanics.EnsureRegistered(player);
            if (!registered)
            {
                Console.WriteLine($"Player {player.Name} is not registered.");
                return;
            }
            Console.WriteLine($"Player {player.Name} is registered.");
            var notBanned = PlayerMechanics.EnsureNotBanned(player);
            if (!notBanned)
            {
                Console.WriteLine($"Player {player.Name} is banned.");
                return;
            }
            Console.WriteLine($"Player {player.Name} is not banned.");

            player.SendChatMessage("Enter your password to login:");
            PlayerMechanics.PlayersOnPasswordEntry.Add(player);
        }

        [ServerEvent(Event.PlayerSpawn)]
        public void OnPlayerSpawn(Player player)
        {
            Console.WriteLine($"Spawning player {player.Name}");
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
            if (PlayerMechanics.PlayersOnPasswordEntry.Contains(player))
            {
                var result = PlayerMechanics.SavePlayerData(player);
                if (!result)
                {
                    Console.WriteLine($"Error saving player {player.Name} data");
                }
                PlayerMechanics.PlayersOnPasswordEntry.Remove(player);
            }

            var playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();

            if (playerEntity == null)
            {
                Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                throw new Exception($"Player {player.Name} not found in database during disconnection.");
            }

            playerEntity.IsOnline = false;

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

        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Player player, string message)
        {
            if (PlayerMechanics.PlayersOnPasswordEntry.Contains(player))
            {
                var playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
                if (playerEntity == null)
                {
                    Console.WriteLine($"FATAL: Player {player.Name} not found in database during password entry.");
                    throw new Exception($"Player {player.Name} not found in database during password entry.");
                }
                if (playerEntity.Position == null)
                {
                    Console.WriteLine($"FATAL: Player {player.Name} has no position set.");
                    throw new Exception($"Player {player.Name} has no position set.");
                }

                if (playerEntity.Password == message)
                {
                    player.SendChatMessage("Password verified. Welcome back!");
                    PlayerMechanics.PlayersOnPasswordEntry.Remove(player);
                    playerEntity.IsOnline = true;
                    NAPI.Entity.SetEntityTransparency(player, 255);
                    NAPI.Entity.SetEntityDimension(player, 0);

                    Vector3 spawnPosition = new Vector3(playerEntity.Position.X, playerEntity.Position.Y, playerEntity.Position.Z);
                    NAPI.Player.SpawnPlayer(player, spawnPosition, playerEntity.Position.Heading);
                }
                else
                {
                    player.SendChatMessage("Incorrect password. Please try again.");
                }

                return;
            }
        }
    }
}
