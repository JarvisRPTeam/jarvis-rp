using GTANetworkAPI;
using GameDb.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Main.AdminPanel
{
    public class AdminPanel : Script
    {
        [RemoteEvent("adminPanel:requestPlayers")]
        public void RequestPlayers(Player player)
        {
            var players = GameDbContainer.PlayerService.GetAllPlayersAsync().GetAwaiter().GetResult();

            var onlinePlayers = new List<PlayerInfoModel>();
            var offlinePlayers = new List<PlayerInfoModel>();
            var bannedPlayers = new List<PlayerInfoModel>();
            var onlineAdmins = new List<PlayerInfoModel>();
            var offlineAdmins = new List<PlayerInfoModel>();
            var bannedAdmins = new List<PlayerInfoModel>();
            var onlineServerOwners = new List<PlayerInfoModel>();
            var offlineServerOwners = new List<PlayerInfoModel>();

            foreach (var p in players)
            {
                var info = new PlayerInfoModel
                {
                    Id = p.Id,
                    Nickname = p.Nickname,
                    Role = p.Role.Name,
                    IsOnline = p.IsOnline
                };

                // Check for active punishments
                bool hasActivePunishment = false;
                if (p.Punishments != null)
                {
                    foreach (var punishment in p.Punishments)
                    {
                        if (punishment.Timeout == null || punishment.Timeout > DateTime.UtcNow)
                        {
                            hasActivePunishment = true;
                            break;
                        }
                    }
                }

                // Categorize by role and online status, exclude banned from online/offline lists
                if (p.Role.Name == "Admin")
                {
                    if (hasActivePunishment)
                        bannedAdmins.Add(info);
                    else if (p.IsOnline)
                        onlineAdmins.Add(info);
                    else
                        offlineAdmins.Add(info);
                }
                else if (p.Role.Name == "ServerOwner")
                {
                    // ServerOwners are not banned, keep original logic
                    if (p.IsOnline)
                        onlineServerOwners.Add(info);
                    else
                        offlineServerOwners.Add(info);
                }
                else
                {
                    if (hasActivePunishment)
                        bannedPlayers.Add(info);
                    else if (p.IsOnline)
                        onlinePlayers.Add(info);
                    else
                        offlinePlayers.Add(info);
                }
            }

            var panelModel = new PlayerPanelModel
            {
                OnlinePlayers = onlinePlayers,
                OfflinePlayers = offlinePlayers,
                OnlineAdmins = onlineAdmins,
                OfflineAdmins = offlineAdmins,
                OnlineServerOwners = onlineServerOwners,
                OfflineServerOwners = offlineServerOwners,
                BannedPlayers = bannedPlayers,
                BannedAdmins = bannedAdmins
            };

            string json = JsonConvert.SerializeObject(panelModel);
            player.TriggerEvent("adminPanel:receivePlayers", json);
        }

        // [RemoteEvent("adminPanel:requestVehicles")]
        // public void RequestVehicles(Player player)
        // {
        //     // Fetch all vehicles from your DB/service
        //     var vehicles = GameDbContainer.VehicleService.GetAllVehiclesAsync().GetAwaiter().GetResult();

        //     var onlineVehicles = new List<VehicleInfoModel>();
        //     var offlineVehicles = new List<VehicleInfoModel>();
        //     var bannedVehicles = new List<VehicleInfoModel>();

        //     foreach (var v in vehicles)
        //     {
        //         var info = new VehicleInfoModel
        //         {
        //             Id = (int)v.Id,
        //             Model = v.Model,
        //             Owner = v.Owner != null ? v.Owner.Nickname : "Unknown",
                 
        //         };

        //         if (info.IsBanned)
        //         {
        //             bannedVehicles.Add(info);
        //         }
        //         else if (info.IsOnline)
        //         {
        //             onlineVehicles.Add(info);
        //         }
        //         else
        //         {
        //             offlineVehicles.Add(info);
        //         }
        //     }

        //     var panelModel = new VehiclePanelModel
        //     {
        //         OnlineVehicles = onlineVehicles,
        //         OfflineVehicles = offlineVehicles,
        //         BannedVehicles = bannedVehicles
        //     };

        //     string json = JsonConvert.SerializeObject(panelModel);
        //     player.TriggerEvent("adminPanel:receiveVehicles", json);
        // }


        
    }
}