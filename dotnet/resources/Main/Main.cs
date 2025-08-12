using GTANetworkAPI;
using GameDb.Service;
using System;
using GameDb.Domain.Models;

namespace Main
{
    public class Main: Script
    {
        public Main()
        {
            if (GameDbContainer.IsReady().GetAwaiter().GetResult())
            {
                NAPI.Util.ConsoleOutput("GameDbContainer initialized.");

                var result = GameDbContainer.PlayerService.SetAllPlayersOfflineAsync().GetAwaiter().GetResult();
                if (!result)
                {
                    NAPI.Util.ConsoleOutput("Failed to set all players offline.");
                }
            }
            else
            {
                throw new InvalidOperationException("GameDbContainer initialization failed");
            }
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.Server.SetGlobalServerChat(false);

            NAPI.Util.ConsoleOutput("Main started.");
        }
    }
}
