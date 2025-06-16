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

       


        [RemoteEvent("sendSpeedToServer")]
        public void OnSpeedEvent(Player player, int speed)
        {
            NAPI.Util.ConsoleOutput($"[Speed] {player.Name}: {speed} km/h");
        }

    }
}
