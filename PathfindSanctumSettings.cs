using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;
using Color = SharpDX.Color;

namespace PathfindSanctum;

public class PathfindSanctumSettings : ISettings
{
    private static readonly IReadOnlyDictionary<string, string> AfflictionDescriptions = new Dictionary<string, string>
    {
        { "Accursed Prism", "When you gain an Affliction, gain an additional random Minor Affliction" },
        { "Anomaly Attractor", "Rooms spawn Volatile Anomalies" },
        { "Black Smoke", "You can see one fewer room ahead on the Sanctum Map" },
        { "Blunt Sword", "You and your Minions deal 25% less Damage" },
        { "Chains of Binding", "Monsters inflict Binding Chains on Hit" },
        { "Charred Coin", "50% less Aureus coins found" },
        { "Chiselled Stone", "Monsters Petrify on Hit" },
        { "Concealed Anomaly", "Guards release a Volatile Anomaly on Death" },
        { "Corrosive Concoction", "No Resolve Mitigation, chance to Avoid Resolve loss or Resolve Aegis" },
        { "Corrupted Lockpick", "Chests in rooms explode when opened" },
        { "Cutpurse", "You cannot gain Aureus coins" },
        { "Dark Pit", "Traps impact 100% increased Resolve" },
        { "Deadly Snare", "Traps impact infinite Resolve" },
        { "Death Toll", "Monsters no longer drop Aureus coins" },
        { "Deceptive Mirror", "You are not always taken to the room you select" },
        { "Demonic Skull", "Cannot recover Resolve" },
        { "Door Tax", "Lose 30 Aureus coins on room completion" },
        { "Empty Trove", "Chests no longer drop Aureus coins" },
        { "Fiendish Wings", "Monsters' Action Speed cannot be slowed below base, Monsters have 30% increased Attack, Cast and Movement Speed" },
        { "Floor Tax", "Lose all Aureus on floor completion" },
        { "Gargoyle Totem", "Guards are accompanied by a Gargoyle" },
        { "Ghastly Scythe", "Losing Resolve ends your Sanctum" },
        { "Glass Shard", "The next Boon you gain is converted into a random Minor Affliction" },
        { "Golden Smoke", "Rewards are unknown on the Sanctum Map" },
        { "Haemorrhage", "You cannot recover Resolve (removed after killing the next Floor Boss)" },
        { "Honed Claws", "Monsters deal 25% more Damage" },
        { "Hungry Fangs", "Monsters impact 25% increased Resolve" },
        { "Iron Manacles", "Cannot Avoid Resolve Loss from Enemy Hits" },
        { "Liquid Cowardice", "Lose 10 Resolve when you use a Flask" },
        { "Mark of Terror", "Monsters inflict Resolve Weakness on Hit" },
        { "Orb of Negation", "Relics have no Effect" },
        { "Phantom Illusion", "Every room grants a random Minor Affliction, Afflictions granted this way are removed on room completion" },
        { "Poisoned Water", "Gain a random Minor Affliction when you use a Fountain" },
        { "Purple Smoke", "Afflictions are unknown on the Sanctum Map" },
        { "Rapid Quicksand", "Traps are faster" },
        { "Red Smoke", "Room types are unknown on the Sanctum Map" },
        { "Rusted Coin", "The Merchant only offers one choice" },
        { "Rusted Mallet", "Monsters always Knockback, Monsters have increased Knockback Distance" },
        { "Sharpened Arrowhead", "Enemy Hits ignore your Resolve Mitigation" },
        { "Shattered Shield", "Cannot have Resolve Aegis" },
        { "Spiked Exit", "Lose 5% of current Resolve on room completion" },
        { "Spiked Shell", "Monsters have 30% increased Maximum Life" },
        { "Spilt Purse", "Lose 20 Aureus coins when you lose Resolve from a Hit" },
        { "Tattered Blindfold", "90% reduced Light Radius, Minimap is hidden" },
        { "Tight Choker", "You can have a maximum of 5 Boons" },
        { "Unassuming Brick", "You cannot gain any more Boons" },
        { "Unhallowed Amulet", "The Merchant offers 50% fewer choices" },
        { "Unhallowed Ring", "50% increased Merchant prices" },
        { "Unholy Urn", "50% reduced Effect of your Relics" },
        { "Unquenched Thirst", "50% reduced Resolve recovered" },
        { "Veiled Sight", "Rooms are unknown on the Sanctum Map" },
        { "Voodoo Doll", "100% more Resolve lost while Resolve is below 50%" },
        { "Weakened Flesh", "-100 to Maximum Resolve" },
        { "Worn Sandals", "40% reduced Movement Speed" }
    };


    public PathfindSanctumSettings()
    {
        var currencyFilter = "";
        var fightRoomFilter = "";
        var roomFilter = "";
        var afflictionFilter = "";
        TieringNode = new CustomNode
        {
            DrawDelegate = () =>
            {
                var profileName = CurrentProfile != null && Profiles.ContainsKey(CurrentProfile) ? CurrentProfile : Profiles.Keys.FirstOrDefault() ?? "Default";
                var profile = GetCurrentProfile();
                foreach (var key in Profiles.Keys.OrderBy(x => x).ToList())
                {
                    if (key == profileName)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, Color.Green.ToImgui());
                        ImGui.Button($"{key}##profile");
                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        if (ImGui.Button($"Activate profile {key}##profile"))
                        {
                            CurrentProfile = key; // Works as intended
                        }
                    }
                }

                if (ImGui.Button("Add profile##addProfile"))
                {
                    var newProfileName = Enumerable.Range(0, 100).Select(x => $"New profile {x}").First(x => !Profiles.ContainsKey(x));
                    Profiles[newProfileName] = ProfileContent.CreateDefaultProfile();
                    CurrentProfile = newProfileName;
                }

                if (ImGui.TreeNode("Currency weights"))
                {
                    ImGui.InputTextWithHint("##CurrencyFilter", "Filter", ref currencyFilter, 100);

                    IEnumerable<string> currencyTypes;

                    if (RemoteMemoryObject.pTheGame?.Files?.SanctumDeferredRewardCategories?.EntriesList.Count != 0)
                    {
                        currencyTypes = RemoteMemoryObject.pTheGame.Files.SanctumDeferredRewardCategories.EntriesList
                            .Select(entry => entry.CurrencyName);
                    }
                    else
                    {
                        currencyTypes = profile.CurrencyWeights.Keys
                            .Select(key => key.Split('_')[0])
                            .Distinct();
                    }

                    foreach (var type in currencyTypes.Where(t => t.Contains(currencyFilter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            string suffix = i switch
                            {
                                0 => "_Now",
                                1 => "_EndOfFloor",
                                _ => "_EndOfSanctum"
                            };

                            string rewardType = $"{type}{suffix}";
                            var currentValue = GetCurrencyWeight(rewardType);

                            if (ImGui.InputInt($"{rewardType}", ref currentValue))
                            {
                                profile.CurrencyWeights[rewardType] = currentValue;
                            }
                        }
                    }

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Fight Room weights"))
                {

                    ImGui.InputTextWithHint("##FightFilter", "Filter", ref fightRoomFilter, 100);

                    // We split this into Fight Room & Reward Room, so it isn't a reasonable default value
                    //fightRoomTypes = RemoteMemoryObject.pTheGame.Files.SanctumRoomTypes.EntriesList
                    //    .Select(entry => entry.Id);
                    var fightRoomTypes = profile.FightRoomWeights.Keys
                            .Select(key => key.Split('_')[0])
                            .Distinct();

                    foreach (var type in fightRoomTypes.Where(t => t.Contains(fightRoomFilter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        for (int floor = 1; floor <= 4; floor++)
                        {
                            string floorType = $"{type}_Floor{floor}";
                            var currentValue = GetFightRoomWeight(floorType);

                            if (ImGui.InputInt($"{floorType}", ref currentValue))
                            {
                                profile.FightRoomWeights[floorType] = currentValue;
                            }
                        }
                    }

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Reward Type weights"))
                {
                    ImGui.InputTextWithHint("##RoomFilter", "Filter", ref roomFilter, 100);
                    var rewardRoomTypes = profile.RewardRoomWeights.Keys
                        .Select(key => key.Split('_')[0])
                        .Distinct();

                    foreach (var type in rewardRoomTypes.Where(t => t.Contains(roomFilter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        for (int floor = 1; floor <= 4; floor++)
                        {
                            string floorType = $"{type}_Floor{floor}";
                            var currentValue = GetRewardRoomWeight(floorType);

                            if (ImGui.InputInt($"{floorType}", ref currentValue))
                            {
                                profile.RewardRoomWeights[floorType] = currentValue;
                            }
                        }
                    }

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Affliction weights"))
                {
                    ImGui.InputTextWithHint("##AfflictionFilter", "Filter", ref afflictionFilter, 100);
                    IEnumerable<string> afflictionTypes;

                    if (RemoteMemoryObject.pTheGame?.Files?.SanctumPersistentEffects.EntriesList.Count != 0)
                    {
                        afflictionTypes = RemoteMemoryObject.pTheGame.Files.SanctumPersistentEffects.EntriesList
                            .Where(entry => entry.Id.StartsWith("Affliction"))
                            .Select(entry => entry.ReadableName)
                            .Distinct();
                    }
                    else
                    {
                        afflictionTypes = profile.AfflictionWeights.Keys
                            .Select(key => key.Split('_')[0])
                            .Distinct();
                    }
                    foreach (var type in afflictionTypes.Where(t =>
                                 t.Contains(afflictionFilter, StringComparison.InvariantCultureIgnoreCase) ||
                                 AfflictionDescriptions[t].Contains(afflictionFilter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        var currentValue = GetAfflictionWeight(type);
                        if (ImGui.InputInt($"{type}", ref currentValue))
                        {
                            profile.AfflictionWeights[type] = currentValue;
                        }

                        ImGui.SameLine();
                        ImGui.TextDisabled("(?)");
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip(AfflictionDescriptions[type]);
                        }
                    }

                    ImGui.TreePop();
                }
            }
        };
    }

    public ProfileContent GetCurrentProfile()
    {
        var profileName = CurrentProfile != null && Profiles.ContainsKey(CurrentProfile) ? CurrentProfile : Profiles.Keys.FirstOrDefault() ?? "Default";
        if (!Profiles.TryGetValue(profileName, out ProfileContent value))
        {
            value = ProfileContent.CreateDefaultProfile();
            Profiles[profileName] = value;
        }

        return value;
    }

    public int GetCurrencyWeight(string type)
    {
        // TODO: Get Quantity From Files instead of using 3 weight maps
        var profile = GetCurrentProfile();
        if (type.Contains("Mirror"))
        {
            return 1000000; // Developer safety net
        }

        if (profile.CurrencyWeights.TryGetValue(type, out var weight))
        {
            return weight;
        }



        return 0;
    }

    public int GetFightRoomWeight(string type)
    {
        var profile = GetCurrentProfile();
        if (profile.FightRoomWeights.TryGetValue(type, out int weight))
        {
            return weight;
        }

        return 0;
    }

    public int GetRewardRoomWeight(string type)
    {
        var profile = GetCurrentProfile();
        if (profile.RewardRoomWeights.TryGetValue(type, out int weight))
        {
            return weight;
        }

        return 0;
    }

    public int GetAfflictionWeight(string type)
    {
        var profile = GetCurrentProfile();
        if (profile.AfflictionWeights.TryGetValue(type, out var weight))
        {
            return weight;
        }

        return 0;
    }

    public ToggleNode Enable { get; set; } = new ToggleNode(true);

    public ToggleNode DebugEnable { get; set; } = new ToggleNode(false);

    public ColorNode TextColor { get; set; } = new ColorNode(Color.White);
    public ColorNode BackgroundColor { get; set; } = new ColorNode(Color.Black with { A = 128 });
    public ColorNode BestPathColor { get; set; } = new(Color.Cyan);

    public RangeNode<int> FrameThickness { get; set; } = new RangeNode<int>(5, 0, 10);

    public Dictionary<string, ProfileContent> Profiles = new Dictionary<string, ProfileContent>
    {
        ["Default"] = ProfileContent.CreateDefaultProfile(),
        ["NoHit"] = ProfileContent.CreateNoHitProfile(),
        ["Phrecia"] = ProfileContent.CreatePhreciaProfile()
    };

    public string CurrentProfile;

    [JsonIgnore]
    public CustomNode TieringNode { get; set; }
}