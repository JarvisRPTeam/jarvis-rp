using System;
using GTANetworkAPI;

namespace GreetingTest
{
    public class GreetingTest: Script
    {
        public GreetingTest()
        {
            NAPI.Server.SetGamemodeName("GreetingTest"); // Example: Set the gamemode name
        }

        // Event handler for when a player connects
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            // Output greeting message to the console
            NAPI.Util.ConsoleOutput($"Hello, {player.Name}. Your position is {player.Position}");

            // Get player's position
            Vector3 playerPosition = new Vector3(player.Position.X, player.Position.Y + 5, player.Position.Z + 1);
            
            NAPI.Vehicle.CreateVehicle(VehicleHash.Benson, playerPosition, 0, 0, 0);
            Console.WriteLine($"Created a vehicle at {playerPosition}");
        }
    }
}
