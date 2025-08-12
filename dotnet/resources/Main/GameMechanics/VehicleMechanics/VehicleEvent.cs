using GTANetworkAPI;
using System;
using System.Linq;
using GameDb.Service;
using GameDb.Domain.Models;

namespace GameMechanics
{
    public class VehicleEvent : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            VehicleMechanics.LoadAllVehicles();
        }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seat)
        {
            if (seat != 0) return; // Only driver

            // Store last position and enable mileage tracking flag
            vehicle.SetData("lastPosition", vehicle.Position);
            vehicle.SetData("trackingMileage", true);
        }

        [RemoteEvent("Vehicle_AddDistance")]
        public void OnVehicleAddDistance(Player player, float distance)
        {
            try
            {
                if (player == null || !player.Exists) return;
                if (!player.IsInVehicle || player.VehicleSeat != 0) return;

                Vehicle vehicle = player.Vehicle;
                if (vehicle == null || !vehicle.Exists) return;

                if (distance <= 0 || distance > 10000f)
                {
                    NAPI.Util.ConsoleOutput($"[VehicleEvent] Rejected unrealistic distance from {player.Name}: {distance:F2}m");
                    return;
                }

                float currentMileage = VehicleMechanics.GetMileage(vehicle);
                float distanceKm = distance / 1000f;
                float newMileage = currentMileage + distanceKm;

                vehicle.SetData("mileage", newMileage);

                player.TriggerEvent("Vehicle_UpdateMileageDisplay", newMileage);

                bool saved = VehicleMechanics.SaveVehicleData(vehicle);
                if (!saved)
                {
                    NAPI.Util.ConsoleOutput($"[VehicleEvent] Failed to save mileage for vehicle {vehicle.NumberPlate}");
                }
                else
                {
                    NAPI.Util.ConsoleOutput($"[VehicleEvent] {player.Name} drove {distanceKm:F3} km. Mileage updated from {currentMileage:F2} to {newMileage:F2} km for {vehicle.NumberPlate}");
                }
            }
            catch (Exception ex)
            {
                NAPI.Util.ConsoleOutput($"[VehicleEvent] Exception in Vehicle_AddDistance for {player?.Name}: {ex.Message}\n{ex.StackTrace}");
            }
        }





        [ServerEvent(Event.PlayerExitVehicle)]
        public void OnPlayerExitVehicle(Player player, Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.Exists || player == null || !player.Exists) return;

            // Save vehicle data including mileage
            bool saveResult = VehicleMechanics.SaveVehicleData(vehicle);
            if (!saveResult)
            {
                NAPI.Util.ConsoleOutput($"[VehicleEvent] Failed to save vehicle data for {vehicle.NumberPlate}.");
                player.SendChatMessage("~r~Neizdevās saglabāt transporta datus.");
            }
            else
            {
                float fuel = VehicleMechanics.GetFuel(vehicle);
                NAPI.Util.ConsoleOutput($"[VehicleEvent] {player.Name} exited vehicle {vehicle.NumberPlate}. Fuel saved: {fuel:F1}%.");
            }

            vehicle.ResetData("trackingMileage");
            vehicle.ResetData("lastPosition");
        }
    }
}
