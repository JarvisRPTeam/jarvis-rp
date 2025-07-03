using GTANetworkAPI;

namespace GameMechanics.PlayerMechanics
{
    public class PlayerCommands : Script
    {
        [Command("info")]
        public void GetPlayerInfoCommand(Player player)
        {
            PlayerMechanics.GetPlayerInfo(player);
        }

        [Command("status")]
        public void StatusCommand(Player player)
        {
            PlayerMechanics.PlayerStatus(player);
        }

        [Command("revive")]
        public void RecoverHealthCommand(Player player)
        {
            PlayerMechanics.RecoverFullHealth(player);
            player.SendChatMessage("~g~Your health has been fully restored.");
        }

        [Command("tp")]
        public void TeleportToCoordinates(Player player, float x, float y, float z)
        {
            if (player == null || !player.Exists)
            {
                player.SendChatMessage("~r~Player does not exist.");
                return;
            }

            Vector3 targetPosition = new Vector3(x, y, z);
            PlayerMechanics.TeleportPlayer(player, targetPosition);
            player.SendChatMessage($"~g~Teleported to: {x}, {y}, {z}");
        }



        [Command("eat")]
        public void EatCommand(Player player,int amount)
        {
            PlayerMechanics.EatFood(player,amount);
        }

        [Command("drink")]
        public void DrinkCommand(Player player,int amount)
        {
            PlayerMechanics.DrinkWater(player, amount);
        }

       

        [Command("addmoney")]
        public void AddMoneyCommand(Player player, int amount)
        {
            if (amount <= 0)
            {
                player.SendChatMessage("~r~Amount must be positive.");
                return;
            }
            PlayerMechanics.AddMoney(player, amount);
            player.SendChatMessage($"~g~Added ${amount} to your balance.");
        }

        [Command("removemoney")]
        public void CMD_RemoveMoney(Player player, int amount)
        {
            if (amount <= 0)
            {
                player.SendChatMessage("~r~Invalid amount.");
                return;
            }

            PlayerMechanics.RemoveMoney(player, amount);
            player.SendChatMessage($"~y~Removed {amount}$ from your balance.");
        }

        [Command("money")]
        public void MoneyCommand(Player player)
        {
            long money = PlayerMechanics.GetMoney(player);
            player.SendChatMessage($"~y~Your balance: ${money}");
        }

    }
}
