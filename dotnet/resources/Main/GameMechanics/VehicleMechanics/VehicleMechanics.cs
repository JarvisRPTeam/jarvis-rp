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

        public static bool SaveVehicleData(Vehicle vehicle)
        {
            try
            {
                if (vehicle == null || !vehicle.Exists)
                {
                    NAPI.Util.ConsoleOutput("[VehicleMechanics] Cannot save data — vehicle is null or doesn't exist.");
                    return false;
                }

                if (string.IsNullOrEmpty(vehicle.NumberPlate))
                {
                    NAPI.Util.ConsoleOutput("[VehicleMechanics] Cannot save data — vehicle number plate is null or empty.");
                    return false;
                }

                // Get vehicle from DB
                var vehiclesResult = GameDbContainer.VehicleService
                    .GetVehiclesByNumberPlateAsync(vehicle.NumberPlate)
                    .GetAwaiter().GetResult();

                if (vehiclesResult == null || !vehiclesResult.Any())
                {
                    NAPI.Util.ConsoleOutput($"[VehicleMechanics] Vehicle with plate {vehicle.NumberPlate} not found in DB.");
                    return false;
                }

                var vehicleEntity = vehiclesResult.First();
                var vehicleId = vehicleEntity.Id; // store ID to use later

                if (vehicle.Position == null)
                {
                    NAPI.Util.ConsoleOutput($"[VehicleMechanics] Vehicle {vehicle.NumberPlate} position is invalid.");
                    return false;
                }

                // Save Position
                var position = new PositionModel
                {
                    X = vehicle.Position.X,
                    Y = vehicle.Position.Y,
                    Z = vehicle.Position.Z,
                    Heading = vehicle.Rotation.Z
                };

                bool positionSaved = GameDbContainer.VehicleService
                    .SetPositionAsync(vehicleEntity, position)
                    .GetAwaiter().GetResult();

                if (!positionSaved)
                {
                    NAPI.Util.ConsoleOutput($"[VehicleMechanics] Failed to save position for vehicle {vehicle.NumberPlate}.");
                    return false;
                }

                // Save Color
                var color = new VehicleColorModel
                {
                    PrimaryColor = vehicle.PrimaryColor,
                    SecondaryColor = vehicle.SecondaryColor
                };

                bool colorSaved = GameDbContainer.VehicleService
                    .SetColorAsync(vehicleEntity, color)
                    .GetAwaiter()
                    .GetResult();

                if (!colorSaved)
                {
                    NAPI.Util.ConsoleOutput($"[VehicleMechanics] Failed to save color for vehicle {vehicle.NumberPlate}.");
                    return false;
                }

                // Save Fuel
                float fuel = GetFuel(vehicle);

                bool fuelSaved = GameDbContainer.VehicleService
                    .SetFuelAsync(vehicleEntity, fuel)
                    .GetAwaiter()
                    .GetResult();

                if (!fuelSaved)
                {
                    NAPI.Util.ConsoleOutput($"[VehicleMechanics] Failed to save fuel for vehicle {vehicle.NumberPlate}.");
                    return false;
                }

                // Save Mileage (only if changed)
                float mileage = GetMileage(vehicle);
                NAPI.Util.ConsoleOutput($"[DEBUG] Saving mileage for {vehicle.NumberPlate}: {mileage} km");

                if (Math.Abs(vehicleEntity.Mileage - mileage) > 0.001f)
                {
                    bool mileageSaved = GameDbContainer.VehicleService
                        .SetMileageAsync(vehicleEntity, mileage)
                        .GetAwaiter()
                        .GetResult();

                    if (!mileageSaved)
                    {
                        NAPI.Util.ConsoleOutput($"[VehicleMechanics] Mileage save FAILED for {vehicle.NumberPlate}.");
                        return false;
                    }
                }
                else
                {
                    NAPI.Util.ConsoleOutput($"[VehicleMechanics] Mileage unchanged for {vehicle.NumberPlate}, skipping save.");
                }

                NAPI.Util.ConsoleOutput($"[VehicleMechanics] Successfully saved data for vehicle {vehicle.NumberPlate}.");
                return true;
            }
            catch (Exception ex)
            {
                var plateInfo = vehicle != null ? vehicle.NumberPlate : "unknown";
                NAPI.Util.ConsoleOutput($"[VehicleMechanics] Exception while saving vehicle data (plate: {plateInfo}): {ex.Message}\n{ex.StackTrace}");
                return false;
            }
        }


        public static void LoadAllVehicles()
        {
            var allVehicles = GameDbContainer.VehicleService.GetAllVehiclesAsync().GetAwaiter().GetResult();

            foreach (var vehicleEntity in allVehicles)
            {
                Vehicle vehicle = NAPI.Vehicle.CreateVehicle(
                    (VehicleHash)NAPI.Util.GetHashKey(vehicleEntity.Model),
                    new Vector3(vehicleEntity.Position.X, vehicleEntity.Position.Y, vehicleEntity.Position.Z),
                    vehicleEntity.Position.Heading,
                    vehicleEntity.Color.PrimaryColor,
                    vehicleEntity.Color.SecondaryColor
                );

                vehicle.NumberPlate = vehicleEntity.NumberPlate;
                vehicle.Locked = false;
                vehicle.EngineStatus = true;

                // Set mileage on vehicle instance:
                vehicle.SetData("mileage", vehicleEntity.Mileage);

                NAPI.Util.ConsoleOutput($"[VehicleMechanics] Spawned vehicle {vehicle.NumberPlate} with mileage {vehicleEntity.Mileage} km");
            }
        }


        public static bool RemoveVehicle(string numberPlate)
        {
            if (string.IsNullOrEmpty(numberPlate))
            {
                NAPI.Util.ConsoleOutput("[VehicleMechanics] Empty plate provided.");
                return false;
            }

            // Get vehicle from database
            var vehicleEntity = GameDbContainer.VehicleService
                .GetVehiclesByNumberPlateAsync(numberPlate)
                .GetAwaiter().GetResult()
                .FirstOrDefault();

            if (vehicleEntity == null)
            {
                NAPI.Util.ConsoleOutput($"[VehicleMechanics] Vehicle with plate {numberPlate} not found in database.");
                return false;
            }

            // Try to find and delete in-game vehicle
            var vehicle = NAPI.Pools.GetAllVehicles().FirstOrDefault(v => v.NumberPlate == numberPlate);
            if (vehicle != null && vehicle.Exists)
            {
                NAPI.Entity.DeleteEntity(vehicle);
            }

            // Delete from DB
            var result = GameDbContainer.VehicleService.RemoveVehicleAsync(vehicleEntity).GetAwaiter().GetResult();
            if (!result)
            {
                NAPI.Util.ConsoleOutput($"[VehicleMechanics] Failed to delete vehicle {numberPlate} from database.");
                return false;
            }

            NAPI.Util.ConsoleOutput($"[VehicleMechanics] Vehicle {numberPlate} removed successfully.");
            return true;
        }



        public static Vehicle SpawnVehicleForPlayer(Player player, string modelName, string plateText = "")
        {
            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentException("Model name cannot be empty");

            uint model = NAPI.Util.GetHashKey(modelName);
            if (model == 0)
                throw new ArgumentException("Invalid vehicle model name");

            Vector3 spawnPos = CalculateSpawnPosition(player.Position, player.Heading, 5f);


            // Generate random colors for primary and secondary
            var rand = new Random();
            int primaryColor = rand.Next(0, 160); // GTA V supports 0-159 for vehicle colors
            int secondaryColor = rand.Next(0, 160);

            var vehicleEntity = GameDbContainer.VehicleService.CreateVehicleAsync(new VehicleCreateModel
            {
                Model = modelName,
                Position = new PositionModel
                {
                    X = spawnPos.X,
                    Y = spawnPos.Y,
                    Z = spawnPos.Z,
                    Heading = player.Heading
                },
                Color = new VehicleColorModel
                {
                    PrimaryColor = primaryColor,
                    SecondaryColor = secondaryColor
                },
            }).GetAwaiter().GetResult();

            if (vehicleEntity == null)
            {
                player.SendChatMessage("~r~Failed to create vehicle in database.");
                return null;
            }
            string plate = vehicleEntity.NumberPlate;
            player.SendChatMessage($"~g~Vehicle created with plate: {plate}");


            Vehicle vehicle = NAPI.Vehicle.CreateVehicle(
                model,
                spawnPos,
                player.Heading,
                primaryColor,
                secondaryColor,
                plate
            );

            var playerEntity = GameDbContainer.PlayerService.GetPlayerByNicknameAsync(player.Name).GetAwaiter().GetResult();
            if (playerEntity == null)
            {
                Console.WriteLine($"FATAL: Player {player.Name} not found in database.");
                throw new Exception($"Player {player.Name} not found in database.");
            }
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


        //VEHICLE MILLAGE SYSTEM
        public static float GetMileage(Vehicle vehicle)
        {
            if (vehicle.HasData("mileage"))
                return vehicle.GetData<float>("mileage");
            return 0f;
        }


        public static void SetMileage(Vehicle vehicle, float mileage)
        {
            vehicle.SetData("mileage", mileage);
        }



    }
}