using System;
using GTANetworkAPI;
using GameDb.Service;
using GameDb.Repository;

namespace Main
{
    public class Main: Script
    {
        private readonly PlayerService _playerService;

        public Main()
        {
            NAPI.Util.ConsoleOutput("Main Started with GameDbService.");
        }
    }
}
