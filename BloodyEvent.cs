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
using Steamworks;
using Random = System.Random;
using OpenMod.Unturned.Players.Useables.Events;
using OpenMod.Unturned.Players.Inventory.Events;

namespace MyOpenModPlugin
{
    public class BloodyEvent : IEventListener<UnturnedZombieDyingEvent>, IEventListener<UnturnedPlayerHealthUpdatedEvent>, 
        IEventListener<UnturnedPlayerSpawnedEvent>, IEventListener<UnturnedPlayerDeathEvent>
    {

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
        private static HashSet<UnturnedPlayer> bleedingPlayers = new HashSet<UnturnedPlayer>();

        private static Dictionary<CSteamID, int> medicalDropRateByPlayer = new Dictionary<CSteamID, int>();

        private static readonly Random randomValue = new Random();

        private static readonly Guid guid = Guid.Parse("67a4addd45174d7e9ca5c8ec24f8010f");

        public Task HandleEventAsync(object? sender, UnturnedZombieDyingEvent e)
        {
            if (e.Instigator != null)
            {
                UnturnedPlayer player = e.Instigator;
                if (bleedingPlayers.Contains(player) && player.PlayerLife.isBleeding) {
                    String speciality = e.Zombie.Zombie.speciality.ToString();
                    if (Enum.TryParse(speciality, true, out EZombieSpeciality zombieSpeciality))
                    {
                        ushort itemId = zombieItemIdBySpeciality[zombieSpeciality];
                        Item item = new Item(itemId, true);
                        medicalDropRateByPlayer.TryGetValue(player.SteamId, out int currentDropRate);
                        if(randomValue.Next(100) < currentDropRate)
                        {
                            ItemManager.dropItem(item, player.Player.transform.position, true, true, false);
                        }
                        medicalDropRateByPlayer.Remove(player.SteamId);
                        EffectManager.ClearEffectByGuid(guid, player.Player.channel.owner.transportConnection);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private float hallucinateDelta = 0;

        public Task HandleEventAsync(object? sender, UnturnedPlayerHealthUpdatedEvent e)
        {
            UnturnedPlayer player = e.Player;

            if(player.Player.life.isBleeding)
            {
                bleedingPlayers.Add(player);
                player.PrintMessageAsync($"Time left to live: {player.Health} seconds.");

                TriggerEffectParameters tep = new TriggerEffectParameters(guid);
                tep.position = player.Player.transform.position;
                EffectManager.triggerEffect(tep);

                player.Player.life.serverModifyHallucination(hallucinateDelta += 1);

                if (medicalDropRateByPlayer.TryGetValue(player.SteamId, out int existingRate))
                {
                    int newRate = ++existingRate;
                    medicalDropRateByPlayer[player.SteamId] = newRate;
                } else
                {
                    medicalDropRateByPlayer.Add(player.SteamId, 10);
                }
            }   
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerSpawnedEvent e)
        {
            e.Player.Player.inventory.tryAddItem(new Item(16, true), true);
            e.Player.Player.movement.sendPluginSpeedMultiplier(3);
            return Task.CompletedTask;
        }

        public Task HandleEventAsync(object? sender, UnturnedPlayerDeathEvent e)
        {
            if (e.Player != null)
            {
                UnturnedPlayer player = e.Player;
                bleedingPlayers.Remove(player);
                medicalDropRateByPlayer.Remove(player.SteamId);
                EffectManager.ClearEffectByGuid(guid, player.Player.channel.owner.transportConnection);
            }

            return Task.CompletedTask;
          
        }

    }
}
