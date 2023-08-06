using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenMod.API.Eventing;
using OpenMod.Unturned.Events;
using OpenMod.Unturned.Players;
using OpenMod.Unturned.Players.Stats.Events;
using OpenMod.Unturned.Zombies.Events;
using SDG.Unturned;

namespace MyOpenModPlugin
{
    public class BloodyEvent : IEventListener<UnturnedPlayerBleedingUpdatedEvent>, IEventListener<UnturnedZombieDeadEvent>
    {

        private bool isPlayerBleeding = false;

        public Task HandleEventAsync(object? sender, UnturnedPlayerBleedingUpdatedEvent e)
        {
            isPlayerBleeding = e.IsBleeding;
            return Task.CompletedTask;

        }

        public Task HandleEventAsync(object? sender, UnturnedZombieDeadEvent e)
        {
            //EffectManager.sendUIEffect()
            return Task.CompletedTask;
        }
    }
}
