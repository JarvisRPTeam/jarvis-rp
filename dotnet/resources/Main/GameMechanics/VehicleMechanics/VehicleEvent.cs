using GTANetworkAPI;
using GameMechanics;
using System;

namespace GameMechanics
{
    public class VehicleEvent : Script
    {
        // [ServerEvent(Event.ResourceStart)]
        // public void OnResourceStart()
        // {
           
        // }

        [ServerEvent(Event.PlayerEnterVehicle)]
        public void OnPlayerEnterVehicle(Player player, Vehicle vehicle, sbyte seat)
        {
            if (vehicle == null || !vehicle.Exists) return;            
            bool engineStatus = vehicle.EngineStatus;
            string engineState = engineStatus ? "~g~ON" : "~r~OFF"; 
        }

        [ServerEvent(Event.VehicleDeath)]
        public void OnVehicleDeath(Vehicle vehicle)
        {
            if (vehicle == null || !vehicle.Exists) return;
            NAPI.Util.ConsoleOutput($"[VehicleEvent] Vehicle {vehicle.NumberPlate} exploded. Fuel reset.");
        }

        
    }
}
