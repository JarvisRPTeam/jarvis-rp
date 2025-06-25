using GTANetworkAPI;

namespace GameMechanics.PlayerMechanics
{
    public class PlayerCommands : Script
    {
        [Command("eat")]
        public void EatCommand(Player player)
        {
            PlayerMechanics.Consume(player, "food");
            PlayerMechanics.UpdateClientStats(player); // Update UI after eating
        }

        [Command("drink")]
        public void DrinkCommand(Player player)
        {
            PlayerMechanics.Consume(player, "drink");
            PlayerMechanics.UpdateClientStats(player); // Update UI after drinking
        }

        [Command("status")]
        public void StatusCommand(Player player)
        {
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : 100;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : 100;
            player.SendChatMessage($"~y~Hunger: {hunger} | Thirst: {thirst}");
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
            long  money = PlayerMechanics.GetMoney(player);
            player.SendChatMessage($"~y~Your balance: ${money}");
        }
    }
}
