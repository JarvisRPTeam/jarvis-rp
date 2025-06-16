using GameDb.Service;
using GTANetworkAPI;
using System;
using System.Linq;  // Added this namespace
using GameDb.Domain.Models;
using GameDb.Domain.Entities;
using System.Threading.Tasks;

namespace GameMechanics
{
    public static class VehicleMechanics
    {
        // Vehicle Spawning (existing)
        public static Vehicle SpawnVehicleForPlayer(Player player, string modelName, string plateText = "")
        {
            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentException("Model name cannot be empty");

            uint model = NAPI.Util.GetHashKey(modelName);
            if (model == 0)
                throw new ArgumentException("Invalid vehicle model name");

            Vector3 spawnPos = CalculateSpawnPosition(player.Position, player.Heading, 5f);


            var result = GameDbContainer.VehicleService.CreateVehicleAsync(new VehicleCreateModel
            {
                Model = modelName,
            }).GetAwaiter().GetResult();

            if (result.ReturnValue == null)
            {
                player.SendChatMessage("~r~Failed to create vehicle in database.");
                return null;
            }
            string plate = result.ReturnValue.NumberPlate;
            player.SendChatMessage($"~g~Vehicle created with plate: {plate}");


            Vehicle vehicle = NAPI.Vehicle.CreateVehicle(
                model,
                spawnPos,
                player.Heading,
                0,
                0,
                plate
            );

            var playerResult = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            if(playerResult.ReturnValue == null)
            {
                player.SendChatMessage("~r~Failed to find player in database.");
                return null;
            }
        
            GameDbContainer.VehicleService.AssignOwnerAsync(
                result.ReturnValue.Id,
                playerResult.ReturnValue.Id
            
            );
            vehicle.SetData("lightsOn", false);
            return vehicle;
        }

        // Vehicle Spawn Position Calculation
        private static Vector3 CalculateSpawnPosition(Vector3 playerPos, float playerHeading, float distance)
        {
            float headingRad = playerHeading * (float)(Math.PI / 180.0);
            Vector3 forward = new Vector3(
                (float)-Math.Sin(headingRad),
                (float)Math.Cos(headingRad),
                0f
            );
            return playerPos + forward * distance;
        }

        // Engine System
        public static bool ToggleVehicleEngine(Vehicle vehicle, Player player)
        {
            if (vehicle == null || !vehicle.Exists)
                throw new ArgumentException("Invalid vehicle");

            if (player == null || !player.Exists)
                throw new ArgumentException("Invalid player");

            if (player.Vehicle != vehicle)
            {
                throw new InvalidOperationException("Player must be in the vehicle to toggle the engine");
            }

            vehicle.EngineStatus = !vehicle.EngineStatus;
            return vehicle.EngineStatus;
        }

        // Lock System
        public static bool ToggleVehicleLock(Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.Exists)
                throw new ArgumentException("Invalid vehicle");

            vehicle.Locked = !vehicle.Locked;
            return vehicle.Locked;
        }

        // Helper method to get player's current or nearest vehicle
        public static Vehicle GetPlayerVehicle(Player player, bool mustBeInside = false)
        {
            if (player.IsInVehicle)
                return player.Vehicle;

            if (mustBeInside)
                return null;
            // Find nearest vehicle if not inside one
            var allVehicles = NAPI.Pools.GetAllVehicles();
            Vehicle nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (var vehicle in allVehicles)
            {
                float distance = vehicle.Position.DistanceTo(player.Position);
                if (distance < 5f && distance < nearestDistance)
                {
                    nearest = vehicle;
                    nearestDistance = distance;
                }
            }
            return nearest;
        }


        //FUEL SYSTEM
        public static void UpdateFuel(Vehicle vehicle, float kmDriven, Player player = null)
        {
            if (vehicle == null) return;

            float fuel = vehicle.HasData("fuelLevel") ? vehicle.GetData<float>("fuelLevel") : 100f;
            float consumptionPerKm = 0.2f; // You can adjust per vehicle type if needed

            fuel -= kmDriven * consumptionPerKm;
            fuel = Math.Clamp(fuel, 0f, 100f);

            vehicle.SetData("fuelLevel", fuel);

            if (fuel <= 0f)
            {
                vehicle.EngineStatus = false;

                if (player != null)
                {
                    player.SendChatMessage("~r~Vehicle ran out of fuel!");
                }
            }
        }

        public static float GetFuel(Vehicle vehicle)
        {
            return vehicle.HasData("fuelLevel") ? vehicle.GetData<float>("fuelLevel") : 100f;
        }

        public static void SetFuel(Vehicle vehicle, float amount)
        {
            vehicle.SetData("fuelLevel", Math.Clamp(amount, 0f, 100f));
        }

        // VEHICLE HEALTH SYSTEM
        public static void SetVehicleHealth(Vehicle vehicle, int health)
        {
            if (vehicle == null) return;
            health = Math.Clamp(health, 0, 1000);
            vehicle.Health = (ushort)health;
        }

        public static void RepairVehicle(Vehicle vehicle)
        {
            if (vehicle == null) return;
            vehicle.Repair();
            vehicle.Health = 1000;
        }




    }
}