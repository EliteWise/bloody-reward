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
using OpenMod.Unturned.Users;
using OpenMod.Extensions.Games.Abstractions.Entities;
using System.Numerics;
using OpenMod.Unturned.Entities;

namespace MyOpenModPlugin
{
    public class BloodyEvent : IEventListener<UnturnedPlayerBleedingUpdatedEvent>, IEventListener<UnturnedZombieDyingEvent>
    {

        private bool isPlayerBleeding = false;
        private Dictionary<EZombieSpeciality, ushort> zombieItemIdBySpeciality = new Dictionary<EZombieSpeciality, ushort>
        {
            { EZombieSpeciality.NORMAL, 95 },
            { EZombieSpeciality.BURNER, 15 },
            { EZombieSpeciality.FLANKER_STALK, 389 },
            { EZombieSpeciality.CRAWLER, 391 },
            { EZombieSpeciality.ACID, 392 },
            { EZombieSpeciality.SPRINTER, 393 },
            { EZombieSpeciality.SPIRIT, 395 }
        };



        public Task HandleEventAsync(object? sender, UnturnedPlayerBleedingUpdatedEvent e)
        {
            isPlayerBleeding = e.IsBleeding;
            return Task.CompletedTask;

        }

        public async Task HandleEventAsync(object? sender, UnturnedZombieDyingEvent e)
        {
            if (e.Instigator != null)
            {
                UnturnedPlayer player = e.Instigator;
                String speciality = e.Zombie.Zombie.speciality.ToString();
                await player.PrintMessageAsync($"Data: {player.Player.name} - {speciality}");
                if(Enum.TryParse(e.Zombie.Zombie.name, true, out EZombieSpeciality zombieSpeciality))
                {
                    ushort itemId = zombieItemIdBySpeciality[zombieSpeciality];
                    Item item = new Item(itemId, true);
                    ItemManager.dropItem(item, player.Player.transform.position, true, true, false);
                }
            }

            //EffectManager.sendUIEffect()
            await Task.CompletedTask;
        }
    }
}
