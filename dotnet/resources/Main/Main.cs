using GTANetworkAPI;
using GameDb.Service;
using System;
using RAGE;
using GameMechanics;

namespace Main
{
    public class Main : Script
    {
        public Main()
        {
            if (GameDbContainer.IsReady())
            {
                NAPI.Util.ConsoleOutput("GameDbContainer initialized.");
            }
            else
            {
                throw new InvalidOperationException("GameDbContainer initialization failed. Halting execution.");
            }
            NAPI.Util.ConsoleOutput("Main started.");
        }
    }
}
