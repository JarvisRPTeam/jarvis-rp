using GTANetworkAPI;

namespace GameMechanics.PlayerMechanics
{
    public class PlayerCommands : Script
    {
        [Command("eat")]
        public void EatCommand(Player player)
        {
            PlayerMechanics.Consume(player, "food");
        }


        [Command("drink")]
        public void DrinkCommand(Player player)
        {
            PlayerMechanics.Consume(player, "drink");
        }

        [Command("status")]
        public void StatusCommand(Player player)
        {
            int hunger = player.HasSharedData("Hunger") ? player.GetSharedData<int>("Hunger") : 100;
            int thirst = player.HasSharedData("Thirst") ? player.GetSharedData<int>("Thirst") : 100;
            player.SendChatMessage($"~y~Hunger: {hunger} | Thirst: {thirst}");
        }
    }
}