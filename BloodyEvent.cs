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
using System.Collections.Concurrent;
using static SDG.Provider.SteamGetInventoryResponse;
using Item = SDG.Unturned.Item;
using HarmonyLib;
using OpenMod.Unturned.Players.Life.Events;
using SDG.NetTransport;
using NuGet.Protocol.Plugins;
using System.Xml.Linq;
using YamlDotNet.Core.Tokens;
using UnityEngine;
using OpenMod.Extensions.Games.Abstractions.Players;
using Serilog;

namespace MyOpenModPlugin
{
    public class BloodyEvent : IEventListener<UnturnedZombieDyingEvent>, 
        IEventListener<UnturnedPlayerHealthUpdatedEvent>, IEventListener<UnturnedPlayerSpawnedEvent>
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
        public static HashSet<UnturnedPlayer> bleedingPlayers = new HashSet<UnturnedPlayer>();

        public Task HandleEventAsync(object? sender, UnturnedZombieDyingEvent e)
        {
            if (e.Instigator != null)
            {
                UnturnedPlayer player = e.Instigator;
                if (bleedingPlayers.Contains(player)) {
                    String speciality = e.Zombie.Zombie.speciality.ToString();
                    if (Enum.TryParse(speciality, true, out EZombieSpeciality zombieSpeciality))
                    {
                        ushort itemId = zombieItemIdBySpeciality[zombieSpeciality];
                        Item item = new Item(itemId, true);
                        ItemManager.dropItem(item, player.Player.transform.position, true, true, false);
                        bleedingPlayers.Remove(player);
                    }
                }
            }

            return Task.CompletedTask;
        }

        float hallucinate = 0;

        public Task HandleEventAsync(object? sender, UnturnedPlayerHealthUpdatedEvent e)
        {
            UnturnedPlayer player = e.Player;

            if(player.Player.life.isBleeding)
            {
                bleedingPlayers.Add(player);
                player.PrintMessageAsync($"Time left to live: {player.Health} seconds.");

                Guid guid = Guid.Parse("67a4addd45174d7e9ca5c8ec24f8010f");
                TriggerEffectParameters tep = new TriggerEffectParameters(guid);
                tep.position = player.Player.transform.position;
                EffectManager.triggerEffect(tep);

                player.Player.life.serverModifyHallucination(hallucinate += 1);
            }   
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerSpawnedEvent e)
        {
            e.Player.Player.inventory.tryAddItem(new Item(16, true), true);
            e.Player.Player.movement.sendPluginSpeedMultiplier(3);
            return Task.CompletedTask;
        }
    }
}
