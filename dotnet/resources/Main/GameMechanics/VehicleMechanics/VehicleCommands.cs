//TEMPORAL. WILL BE SOON REMOVED COMMANDS AND REPLACED WITH REMOTE-EVENTS

using GTANetworkAPI;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GameMechanics
{
    public class VehicleManager : Script
    {
        [Command("spawnvehicle")]
        public void CmdSpawnVehicle(Player player, string modelName)
        {
            try
            {
                Vehicle vehicle = VehicleMechanics.SpawnVehicleForPlayer(player, modelName);
                player.SendChatMessage($"~g~Vehicle '{modelName}' spawned successfully.");
            }
            catch (ArgumentException ex)
            {
                player.SendChatMessage($"~r~{ex.Message}");
            }
            catch (Exception ex)
            {
                player.SendChatMessage($"~r~Failed to spawn vehicle: {ex.Message}");
                NAPI.Util.ConsoleOutput($"[Vehicle Spawn Error] {ex}");
            }
        }

        [RemoteEvent("toggleEngine")]
        public void RemoteToggleEngine(Player player)
        {
            try
            {
                Vehicle vehicle = VehicleMechanics.GetPlayerVehicle(player, true);
                if (vehicle == null)
                {
                    player.SendChatMessage("~r~You must be in a vehicle!");
                    return;
                }

                bool newState = VehicleMechanics.ToggleVehicleEngine(vehicle, player);
                player.SendChatMessage($"~g~Engine {(newState ? "~g~ON" : "~r~OFF")}");
            }
            catch (Exception ex)
            {
                player.SendChatMessage($"~r~Error: {ex.Message}");
            }
        }

        // Lock Commands
        [Command("lock")]
        public void CmdToggleLock(Player player)
        {
            try
            {
                Vehicle vehicle = VehicleMechanics.GetPlayerVehicle(player);
                if (vehicle == null)
                {
                    player.SendChatMessage("~r~No vehicle nearby!");
                    return;
                }

                bool newState = VehicleMechanics.ToggleVehicleLock(vehicle);
                player.SendChatMessage($"~g~Vehicle {(newState ? "~r~LOCKED" : "~g~UNLOCKED")}");
            }
            catch (Exception ex)
            {
                player.SendChatMessage($"~r~Error: {ex.Message}");
            }
        }

        [RemoteEvent("toggleLock")]
        public void RemoteToggleLock(Player player)
        {
            try
            {
                Vehicle vehicle = VehicleMechanics.GetPlayerVehicle(player);
                if (vehicle == null)
                {
                    player.SendChatMessage("~r~No vehicle nearby!");
                    return;
                }

                bool newState = VehicleMechanics.ToggleVehicleLock(vehicle);
                player.SendChatMessage($"~g~Vehicle {(newState ? "~r~LOCKED" : "~g~UNLOCKED")}");
            }
            catch (Exception ex)
            {
                player.SendChatMessage($"~r~Error: {ex.Message}");
            }
        }

        // Fuel Commands
        [RemoteEvent("Vehicle_AddDistance")]
        public void AddVehicleDistance(Player player, float distance)
        {
            Vehicle vehicle = player.Vehicle;
            if (vehicle == null || player.VehicleSeat != 0) return;

            float km = distance / 1000f;

            // Update KM
            float total = vehicle.HasData("drivenKM") ? vehicle.GetData<float>("drivenKM") : 0f;
            total += km;
            vehicle.SetData("drivenKM", total);

            // Update Fuel
            VehicleMechanics.UpdateFuel(vehicle, km, player);
        }
        [Command("km")]
        public void CheckVehicleKM(Player player)
        {
            Vehicle vehicle = player.Vehicle;
            if (vehicle == null)
            {
                player.SendChatMessage("~r~You are not in a vehicle.");
                return;
            }

            if (!vehicle.HasData("drivenKM"))
            {
                player.SendChatMessage("~y~This vehicle has not driven any kilometers yet.");
                return;
            }

            float km = vehicle.GetData<float>("drivenKM");
            player.SendChatMessage($"~g~This vehicle has driven ~h~{km:F2} km~s~.");
        }

        [Command("fuel")]
        public void CheckFuel(Player player)
        {
            Vehicle vehicle = player.Vehicle;
            if (vehicle == null)
            {
                player.SendChatMessage("~r~You are not in a vehicle.");
                return;
            }

            float fuel = VehicleMechanics.GetFuel(vehicle);
            player.SendChatMessage($"~b~Fuel level: {fuel:F1} L");
        }

        [Command("refuel")]
        public void RefuelVehicle(Player player)
        {
            Vehicle vehicle = player.Vehicle;
            if (vehicle == null)
            {
                player.SendChatMessage("~r~You are not in a vehicle.");
                return;
            }

            VehicleMechanics.SetFuel(vehicle, 100f);
            player.SendChatMessage("~g~Vehicle refueled to 100%.");
        }

        [Command("setvehhealth")]
        public void CMD_SetVehicleHealth(Player player, int health)
        {
            if (!player.IsInVehicle)
            {
                player.SendChatMessage("~r~You are not in a vehicle.");
                return;
            }

            VehicleMechanics.SetVehicleHealth(player.Vehicle, health);
            player.SendChatMessage($"~g~Vehicle health set to {health}.");
        }

        [Command("repair")]
        public void CMD_RepairVehicle(Player player)
        {
            if (!player.IsInVehicle)
            {
                player.SendChatMessage("~r~You are not in a vehicle.");
                return;
            }

            VehicleMechanics.RepairVehicle(player.Vehicle);
            player.SendChatMessage("~g~Vehicle fully repaired.");
        }


        [Command("deletevehicle")]
        public void CmdDeleteVehicle(Player player, string numberPlate = null)
        {
            if (string.IsNullOrEmpty(numberPlate))
            {
                // No plate provided, try to remove current vehicle
                if (!player.IsInVehicle)
                {
                    player.SendChatMessage("~r~You are not in a vehicle and no plate was specified.");
                    return;
                }

                Vehicle vehicle = player.Vehicle;
                if (vehicle == null || !vehicle.Exists)
                {
                    player.SendChatMessage("~r~No valid vehicle found.");
                    return;
                }

                if (VehicleMechanics.RemoveVehicle(vehicle.NumberPlate))
                {
                    player.SendChatMessage("~g~Vehicle deleted successfully.");
                }
                else
                {
                    player.SendChatMessage("~r~Failed to delete vehicle.");
                }
            }
            else
            {
                // Plate provided
                if (VehicleMechanics.RemoveVehicle(numberPlate))
                {
                    player.SendChatMessage($"~g~Vehicle with plate ~y~{numberPlate} ~g~deleted successfully.");
                }
                else
                {
                    player.SendChatMessage($"~r~Failed to delete vehicle with plate ~y~{numberPlate}.");
                }
            }
        }


        [Command("vstats")]
        public void CmdVehicleStats(Player player)
        {
            Vehicle vehicle = player.Vehicle;

            if (vehicle == null || !vehicle.Exists)
            {
                player.SendChatMessage("~r~You are not in a valid vehicle.");
                return;
            }

            string plate = vehicle.NumberPlate;
            string model = NAPI.Vehicle.GetVehicleDisplayName(vehicle.Model);
            int primaryColor = vehicle.PrimaryColor;
            int secondaryColor = vehicle.SecondaryColor;
            float health = vehicle.Health;
            float fuel = VehicleMechanics.GetFuel(vehicle);
            float heading = vehicle.Rotation.Z;
            // float mileage = vehicle.HasData("mileage") ? vehicle.GetData<float>("mileage") : 0f;
            bool engine = vehicle.EngineStatus;
            bool locked = vehicle.Locked;

            player.SendChatMessage("~g~--- Vehicle Stats ---");
            player.SendChatMessage($"~y~Model:~w~ {model}");
            player.SendChatMessage($"~y~Plate:~w~ {plate}");
            player.SendChatMessage($"~y~Primary Color:~w~ {primaryColor}");
            player.SendChatMessage($"~y~Secondary Color:~w~ {secondaryColor}");
            player.SendChatMessage($"~y~Health:~w~ {health}/1000");
            player.SendChatMessage($"~y~Fuel Level:~w~ {fuel:0.0}%");
            player.SendChatMessage($"~y~Heading:~w~ {heading:0.0}°");
            // player.SendChatMessage($"~y~Mileage:~w~ {mileage:0.0} km");
            player.SendChatMessage($"~y~Engine Status:~w~ {(engine ? "On" : "Off")}");
            player.SendChatMessage($"~y~Locked:~w~ {(locked ? "Yes" : "No")}");
        }

        [RemoteEvent("sendSpeedToServer")]
        public void OnSpeedEvent(Player player, int speed)
        {
            NAPI.Util.ConsoleOutput($"[Speed] {player.Name}: {speed} km/h");
        }


       [RemoteEvent("adminPanel:spawnVehicle")]
        public void SpawnVehicleHandler(Player player, string modelName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(modelName))
                {
                    player.SendChatMessage("~r~Vehicle model name cannot be empty.");
                    return;
                }

                Vehicle vehicle = VehicleMechanics.SpawnVehicleForPlayer(player, modelName);

                if (vehicle == null)
                {
                    player.SendChatMessage("~r~Failed to spawn vehicle.");
                    return;
                }

                player.SendChatMessage($"~g~Vehicle {modelName} spawned successfully.");
            }
            catch (ArgumentException ex)
            {
                player.SendChatMessage($"~r~{ex.Message}");
            }
            catch (Exception ex)
            {
                player.SendChatMessage("~r~An error occurred while spawning the vehicle.");
                Console.WriteLine($"Error in SpawnVehicleHandler: {ex}");
            }
        }

    }
}
