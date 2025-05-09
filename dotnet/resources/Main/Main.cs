using GTANetworkAPI;
using GameDb.Service;

namespace Main
{
    public class Main: Script
    {
        public Main()
        {
            if (GameDbContainer.IsReady()) {
                NAPI.Util.ConsoleOutput("GameDbContainer initialized.");
            } else {
                NAPI.Util.ConsoleOutput("GameDbContainer initialization failed.");
            }
            NAPI.Util.ConsoleOutput("Main started.");
        }
    }
}
