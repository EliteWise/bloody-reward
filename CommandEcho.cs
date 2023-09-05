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
            await PrintAsync("Item is here!");
        }
    }
}
