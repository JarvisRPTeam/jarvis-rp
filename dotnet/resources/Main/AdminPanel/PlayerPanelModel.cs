using System.Collections.Generic;

namespace Main.AdminPanel
{
    public class PlayerPanelModel
    {
        public List<PlayerInfoModel> OnlinePlayers { get; set; }
        public List<PlayerInfoModel> OfflinePlayers { get; set; }
        public List<PlayerInfoModel> BannedPlayers { get; set; }
        public List<PlayerInfoModel> OnlineAdmins { get; set; }
        public List<PlayerInfoModel> OfflineAdmins { get; set; }
        public List<PlayerInfoModel> BannedAdmins { get; set; }
        public List<PlayerInfoModel> OnlineServerOwners { get; set; }
        public List<PlayerInfoModel> OfflineServerOwners { get; set; }
    }
}