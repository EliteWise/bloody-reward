using OpenMod.Core.Commands;
using System;
using System.Threading.Tasks;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using Command = OpenMod.Core.Commands.Command;

namespace MyOpenModPlugin
{
    [Command("awesome")]
    [CommandAlias("awsm")]
    [CommandAlias("aw")]
    [CommandDescription("My awesome command")]
    public class CommandAwesome : Command
    {

        public CommandAwesome(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected async override Task OnExecuteAsync()
        {

            var player = (UnturnedUser)base.Context.Actor;
            var playerTransform = ((UnturnedPlayer)player.Player).Player.transform;
            var position = playerTransform.position + playerTransform.forward * 2;
            var entityId = "1"; // Change this to the ID of the entity you want to spawn

            Item item = new Item(14, true);
            player.Player.Player.inventory.forceAddItem(item, true);

            await PrintAsync("Item is here!");

        }
    }
}
