using GTANetworkAPI;
using GameDb.Service;
using System;

namespace Main
{
    public class Main: Script
    {
        public Main()
        {
            if (GameDbContainer.IsReady().GetAwaiter().GetResult()) {
                NAPI.Util.ConsoleOutput("GameDbContainer initialized.");
            } else {
                throw new InvalidOperationException("GameDbContainer initialization failed. Halting execution.");
            }
            NAPI.Util.ConsoleOutput("Main started.");
        }
    }
}
