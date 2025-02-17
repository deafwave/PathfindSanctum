using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;
using Color = SharpDX.Color;

namespace BetterSanctum;

public class BetterSanctumSettings : ISettings
{
    private static readonly IReadOnlyList<string> CurrencyTypes = new List<string>
    {
        "Armourer's Scraps",
        "Awakened Sextants",
        "Blacksmith's Whetstones",
        "Blessed Orbs",
        "Cartographer's Chisels",
        "Chaos Orbs",
        "Chromatic Orbs",
        "Divine Orbs",
        "Divine Vessels",
        "Enkindling Orbs",
        "Exalted Orbs",
        "Gemcutter's Prisms",
        "Glassblower's Baubles",
        "Instilling Orbs",
        "Jeweller's Orbs",
        "Mirrors of Kalandra",
        "Orbs of Alchemy",
        "Orbs of Alteration",
        "Orbs of Annulment",
        "Orbs of Augmentation",
        "Orbs of Binding",
        "Orbs of Chance",
        "Orbs of Fusing",
        "Orbs of Horizon",
        "Orbs of Regret",
        "Orbs of Scouring",
        "Orbs of Transmutation",
        "Orbs of Unmaking",
        "Regal Orbs",
        "Sacred Orbs",
        "Stacked Decks",
        "Vaal Orbs",
        "Veiled Chaos Orbs",
    };

    private static readonly IReadOnlyList<string> FightTypes = new List<string>
    {
        "Arena",
        "Boss",
        "Explore",
        "Gauntlet",
        "Lair",
        "Maze",
        "Miniboss",
        "Puzzle",
        "Vault",
    };

    private static readonly IReadOnlyList<string> RoomTypes = new List<string>
    {
        "BoonFountain",
        "Boss",
        "CurseFountain",
        "Deal",
        "Deferral",
        "Final",
        "Fountain",
        "Merchant",
        "RainbowFountain",
        "Treasure",
        "TreasureMinor",
    };

    private static readonly IReadOnlyList<(string, string)> AfflictionTypes = new List<(string, string)>
    {
        ("Accursed Prism", "When you gain an Affliction, gain an additional random Minor Affliction"),
        ("Anomaly Attractor", "Rooms spawn Volatile Anomalies"),
        ("Black Smoke", "You can see one fewer room ahead on the Sanctum Map"),
        ("Blunt Sword", "You and your Minions deal 25% less Damage"),
        ("Chains of Binding", "Monsters inflict Binding Chains on Hit"),
        ("Charred Coin", "50% less Aureus coins found"),
        ("Chiselled Stone", "Monsters Petrify on Hit"),
        ("Concealed Anomaly", "Guards release a Volatile Anomaly on Death"),
        ("Corrosive Concoction", "No Resolve Mitigation, chance to Avoid Resolve loss or Resolve Aegis"),
        ("Corrupted Lockpick", "Chests in rooms explode when opened"),
        ("Cutpurse", "You cannot gain Aureus coins"),
        ("Dark Pit", "Traps impact 100% increased Resolve"),
        ("Deadly Snare", "Traps impact infinite Resolve"),
        ("Death Toll", "Monsters no longer drop Aureus coins"),
        ("Deceptive Mirror", "You are not always taken to the room you select"),
        ("Demonic Skull", "Cannot recover Resolve"),
        ("Door Tax", "Lose 30 Aureus coins on room completion"),
        ("Empty Trove", "Chests no longer drop Aureus coins"),
        ("Fiendish Wings", "Monsters' Action Speed cannot be slowed below base, Monsters have 30% increased Attack, Cast and Movement Speed"),
        ("Floor Tax", "Lose all Aureus on floor completion"),
        ("Gargoyle Totem", "Guards are accompanied by a Gargoyle"),
        ("Ghastly Scythe", "Losing Resolve ends your Sanctum"),
        ("Glass Shard", "The next Boon you gain is converted into a random Minor Affliction"),
        ("Golden Smoke", "Rewards are unknown on the Sanctum Map"),
        ("Haemorrhage", "You cannot recover Resolve (removed after killing the next Floor Boss)"),
        ("Honed Claws", "Monsters deal 25% more Damage"),
        ("Hungry Fangs", "Monsters impact 25% increased Resolve"),
        ("Iron Manacles", "Cannot Avoid Resolve Loss from Enemy Hits"),
        ("Liquid Cowardice", "Lose 10 Resolve when you use a Flask"),
        ("Mark of Terror", "Monsters inflict Resolve Weakness on Hit"),
        ("Orb of Negation", "Relics have no Effect"),
        ("Phantom Illusion", "Every room grants a random Minor Affliction, Afflictions granted this way are removed on room completion"),
        ("Poisoned Water", "Gain a random Minor Affliction when you use a Fountain"),
        ("Purple Smoke", "Afflictions are unknown on the Sanctum Map"),
        ("Rapid Quicksand", "Traps are faster"),
        ("Red Smoke", "Room types are unknown on the Sanctum Map"),
        ("Rusted Coin", "The Merchant only offers one choice"),
        ("Rusted Mallet", "Monsters always Knockback, Monsters have increased Knockback Distance"),
        ("Sharpened Arrowhead", "Enemy Hits ignore your Resolve Mitigation"),
        ("Shattered Shield", "Cannot have Resolve Aegis"),
        ("Spiked Exit", "Lose 5% of current Resolve on room completion"),
        ("Spiked Shell", "Monsters have 30% increased Maximum Life"),
        ("Spilt Purse", "Lose 20 Aureus coins when you lose Resolve from a Hit"),
        ("Tattered Blindfold", "90% reduced Light Radius, Minimap is hidden"),
        ("Tight Choker", "You can have a maximum of 5 Boons"),
        ("Unassuming Brick", "You cannot gain any more Boons"),
        ("Unhallowed Amulet", "The Merchant offers 50% fewer choices"),
        ("Unhallowed Ring", "50% increased Merchant prices"),
        ("Unholy Urn", "50% reduced Effect of your Relics"),
        ("Unquenched Thirst", "50% reduced Resolve recovered"),
        ("Veiled Sight", "Rooms are unknown on the Sanctum Map"),
        ("Voodoo Doll", "100% more Resolve lost while Resolve is below 50%"),
        ("Weakened Flesh", "-100 to Maximum Resolve"),
        ("Worn Sandals", "40% reduced Movement Speed"),
    };

    public BetterSanctumSettings()
    {
        var currencyFilter = "";
        var fightRoomFilter = "";
        var roomFilter = "";
        var afflictionFilter = "";
        TieringNode = new CustomNode
        {
            DrawDelegate = () =>
            {
                var (profileName, profile) = GetCurrentProfile();
                foreach (var key in Profiles.Keys.OrderBy(x => x).ToList())
                {
                    if (key == profileName)
                    {
                        ImGui.PushStyleColor(ImGuiCol.Button, Color.Green.ToImgui());
                        var editedKey = key;
                        if (ImGui.InputText("Edit current profile name", ref editedKey, 200))
                        {
                            Profiles[editedKey] = Profiles[key];
                            Profiles.Remove(key);
                        }

                        ImGui.PopStyleColor();
                    }
                    else
                    {
                        if (ImGui.Button($"Activate profile {key}##profile"))
                        {
                            CurrentProfile = key;
                        }
                    }
                }

                if (ImGui.Button("Add profile##addProfile"))
                {
                    var newProfileName = Enumerable.Range(0, 100).Select(x => $"New profile {x}").First(x => !Profiles.ContainsKey(x));
                    Profiles[newProfileName] = new ProfileContent();
                }

                if (ImGui.TreeNode("Currency weights"))
                {
                    ImGui.InputTextWithHint("##CurrencyFilter", "Filter", ref currencyFilter, 100);
                    foreach (var type in CurrencyTypes.Where(t => t.Contains(currencyFilter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            string suffix;
                            if (i == 0)
                                suffix = "_Now";
                            else if (i == 1)
                                suffix = "_EndOfFloor";
                            else
                                suffix = "_EndOfSanctum";

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
                    foreach (var type in FightTypes.Where(t => t.Contains(fightRoomFilter, StringComparison.InvariantCultureIgnoreCase)))
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

                if (ImGui.TreeNode("Room Type weights"))
                {
                    ImGui.InputTextWithHint("##RoomFilter", "Filter", ref roomFilter, 100);
                    foreach (var type in RoomTypes.Where(t => t.Contains(roomFilter, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        for (int floor = 1; floor <= 4; floor++)
                        {
                            string floorType = $"{type}_Floor{floor}";
                            var currentValue = GetRoomTypeWeight(floorType);

                            if (ImGui.InputInt($"{floorType}", ref currentValue))
                            {
                                profile.RoomTypeWeights[floorType] = currentValue;
                            }
                        }
                    }

                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Affliction weights"))
                {
                    ImGui.InputTextWithHint("##AfflictionFilter", "Filter", ref afflictionFilter, 100);
                    foreach (var (type, description) in AfflictionTypes.Where(t =>
                                 t.Item1.Contains(afflictionFilter, StringComparison.InvariantCultureIgnoreCase) ||
                                 t.Item2.Contains(afflictionFilter, StringComparison.InvariantCultureIgnoreCase)))
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
                            ImGui.SetTooltip(description);
                        }
                    }

                    ImGui.TreePop();
                }
            }
        };
    }

    private (string profileName, ProfileContent profile) GetCurrentProfile()
    {
        var profileName = CurrentProfile != null && Profiles.ContainsKey(CurrentProfile) ? CurrentProfile : Profiles.Keys.FirstOrDefault() ?? "Default";
        if (!Profiles.ContainsKey(profileName))
        {
            Profiles[profileName] = new ProfileContent();
        }

        var profile = Profiles[profileName];
        return (profileName, profile);
    }

    public int GetCurrencyWeight(string type)
    {
        // TODO: Get Quantity From Files instead of using 3 weight maps
        var profile = GetCurrentProfile().profile;
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
        var profile = GetCurrentProfile().profile;
        if (profile.FightRoomWeights.TryGetValue(type, out int weight))
        {
            return weight;
        }

        return 0;
    }

    public int GetRoomTypeWeight(string type)
    {
        var profile = GetCurrentProfile().profile;
        if (profile.RoomTypeWeights.TryGetValue(type, out int weight))
        {
            return weight;
        }

        return 0;
    }

    public int GetAfflictionWeight(string type)
    {
        var profile = GetCurrentProfile().profile;
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
    public ColorNode BestPathColor { get; set; } = new(Color.Green);

    public RangeNode<int> FrameThickness { get; set; } = new RangeNode<int>(5, 0, 10);

    public Dictionary<string, ProfileContent> Profiles = new Dictionary<string, ProfileContent>
    {
        ["Default"] = new ProfileContent()
    };

    public string CurrentProfile;

    [JsonIgnore]
    public CustomNode TieringNode { get; set; }
}

public class ProfileContent
{
    public Dictionary<string, int> CurrencyWeights = new()
    {
        ["Armourer's Scraps_Now"] = 1,
        ["Armourer's Scraps_EndOfFloor"] = 1,
        ["Armourer's Scraps_EndOfSanctum"] = 1,
        ["Awakened Sextants_Now"] = 864,
        ["Awakened Sextants_EndOfFloor"] = 864,
        ["Awakened Sextants_EndOfSanctum"] = 2305,
        ["Blacksmith's Whetstones_Now"] = 2,
        ["Blacksmith's Whetstones_EndOfFloor"] = 2,
        ["Blacksmith's Whetstones_EndOfSanctum"] = 2,
        ["Blessed Orbs_Now"] = 193,
        ["Blessed Orbs_EndOfFloor"] = 193,
        ["Blessed Orbs_EndOfSanctum"] = 290,
        ["Cartographer's Chisels_Now"] = 60,
        ["Cartographer's Chisels_EndOfFloor"] = 120,
        ["Cartographer's Chisels_EndOfSanctum"] = 169,
        ["Chaos Orbs_Now"] = 218,
        ["Chaos Orbs_EndOfFloor"] = 436,
        ["Chaos Orbs_EndOfSanctum"] = 611,
        ["Chromatic Orbs_Now"] = 65,
        ["Chromatic Orbs_EndOfFloor"] = 145,
        ["Chromatic Orbs_EndOfSanctum"] = 218,
        ["Divine Orbs_Now"] = 10000,
        ["Divine Orbs_EndOfFloor"] = 10000,
        ["Divine Orbs_EndOfSanctum"] = 20000,
        ["Divine Vessels_Now"] = 122,
        ["Divine Vessels_EndOfFloor"] = 122,
        ["Divine Vessels_EndOfSanctum"] = 366,
        ["Enkindling Orbs_Now"] = 16,
        ["Enkindling Orbs_EndOfFloor"] = 16,
        ["Enkindling Orbs_EndOfSanctum"] = 32,
        ["Exalted Orbs_Now"] = 637,
        ["Exalted Orbs_EndOfFloor"] = 637,
        ["Exalted Orbs_EndOfSanctum"] = 1275,
        ["Gemcutter's Prisms_Now"] = 158,
        ["Gemcutter's Prisms_EndOfFloor"] = 317,
        ["Gemcutter's Prisms_EndOfSanctum"] = 476,
        ["Glassblower's Baubles_Now"] = 157,
        ["Glassblower's Baubles_EndOfFloor"] = 349,
        ["Glassblower's Baubles_EndOfSanctum"] = 524,
        ["Instilling Orbs_Now"] = 79,
        ["Instilling Orbs_EndOfFloor"] = 158,
        ["Instilling Orbs_EndOfSanctum"] = 238,
        ["Jeweller's Orbs_Now"] = 34,
        ["Jeweller's Orbs_EndOfFloor"] = 75,
        ["Jeweller's Orbs_EndOfSanctum"] = 113,
        ["Mirrors of Kalandra_Now"] = 1000000,
        ["Mirrors of Kalandra_EndOfFloor"] = 1000000,
        ["Mirrors of Kalandra_EndOfSanctum"] = 1000000,
        ["Orbs of Alchemy_Now"] = 30,
        ["Orbs of Alchemy_EndOfFloor"] = 71,
        ["Orbs of Alchemy_EndOfSanctum"] = 101,
        ["Orbs of Alteration_Now"] = 47,
        ["Orbs of Alteration_EndOfFloor"] = 106,
        ["Orbs of Alteration_EndOfSanctum"] = 159,
        ["Orbs of Annulment_Now"] = 305,
        ["Orbs of Annulment_EndOfFloor"] = 305,
        ["Orbs of Annulment_EndOfSanctum"] = 611,
        ["Orbs of Augmentation_Now"] = 3,
        ["Orbs of Augmentation_EndOfFloor"] = 3,
        ["Orbs of Augmentation_EndOfSanctum"] = 3,
        ["Orbs of Binding_Now"] = 17,
        ["Orbs of Binding_EndOfFloor"] = 35,
        ["Orbs of Binding_EndOfSanctum"] = 49,
        ["Orbs of Chance_Now"] = 21,
        ["Orbs of Chance_EndOfFloor"] = 48,
        ["Orbs of Chance_EndOfSanctum"] = 72,
        ["Orbs of Fusing_Now"] = 59,
        ["Orbs of Fusing_EndOfFloor"] = 138,
        ["Orbs of Fusing_EndOfSanctum"] = 198,
        ["Orbs of Horizon_Now"] = 36,
        ["Orbs of Horizon_EndOfFloor"] = 72,
        ["Orbs of Horizon_EndOfSanctum"] = 109,
        ["Orbs of Regret_Now"] = 136,
        ["Orbs of Regret_EndOfFloor"] = 272,
        ["Orbs of Regret_EndOfSanctum"] = 382,
        ["Orbs of Scouring_Now"] = 72,
        ["Orbs of Scouring_EndOfFloor"] = 145,
        ["Orbs of Scouring_EndOfSanctum"] = 203,
        ["Orbs of Transmutation_Now"] = 1,
        ["Orbs of Transmutation_EndOfFloor"] = 1,
        ["Orbs of Transmutation_EndOfSanctum"] = 1,
        ["Orbs of Unmaking_Now"] = 227,
        ["Orbs of Unmaking_EndOfFloor"] = 454,
        ["Orbs of Unmaking_EndOfSanctum"] = 681,
        ["Regal Orbs_Now"] = 116,
        ["Regal Orbs_EndOfFloor"] = 233,
        ["Regal Orbs_EndOfSanctum"] = 349,
        ["Sacred Orbs_Now"] = 1004,
        ["Sacred Orbs_EndOfFloor"] = 1004,
        ["Sacred Orbs_EndOfSanctum"] = 2008,
        ["Stacked Decks_Now"] = 436,
        ["Stacked Decks_EndOfFloor"] = 873,
        ["Stacked Decks_EndOfSanctum"] = 1310,
        ["Vaal Orbs_Now"] = 134,
        ["Vaal Orbs_EndOfFloor"] = 268,
        ["Vaal Orbs_EndOfSanctum"] = 402,
        ["Veiled Chaos Orbs_Now"] = 344,
        ["Veiled Chaos Orbs_EndOfFloor"] = 344,
        ["Veiled Chaos Orbs_EndOfSanctum"] = 689,
    };

    public Dictionary<string, int> FightRoomWeights = new()
    {
        ["Arena_Floor1"] = -50,
        ["Arena_Floor2"] = -50,
        ["Arena_Floor3"] = -100,
        ["Arena_Floor4"] = -100,
        ["Explore_Floor1"] = 50,
        ["Explore_Floor2"] = 50,
        ["Explore_Floor3"] = 2,
        ["Explore_Floor4"] = 2,
        ["Gauntlet_Floor1"] = 49,
        ["Gauntlet_Floor2"] = 49,
        ["Gauntlet_Floor3"] = 1,
        ["Gauntlet_Floor4"] = 1,
        ["Lair_Floor1"] = 0,
        ["Lair_Floor2"] = 0,
        ["Lair_Floor3"] = 0,
        ["Lair_Floor4"] = 0,
        ["Miniboss_Floor1"] = 51,
        ["Miniboss_Floor2"] = 51,
        ["Miniboss_Floor3"] = 3,
        ["Miniboss_Floor4"] = 3,

        // Have never been found in gameplay
        ["Boss"] = 99999,
        ["Maze"] = 99999,
        ["Puzzle"] = 99999,
        ["Vault"] = 99999,
    };
    
    public Dictionary<string, int> RoomTypeWeights = new()
    {
        ["BoonFountain_Floor1"] = 500,
        ["BoonFountain_Floor2"] = 500,
        ["BoonFountain_Floor3"] = 25,
        ["BoonFountain_Floor4"] = 25,
        ["CurseFountain_Floor1"] = 0,
        ["CurseFountain_Floor2"] = 0,
        ["CurseFountain_Floor3"] = 0,
        ["CurseFountain_Floor4"] = 0,
        ["Deal_Floor1"] = 300,
        ["Deal_Floor2"] = 300,
        ["Deal_Floor3"] = 690,
        ["Deal_Floor4"] = 2357,
        ["Deferral_Floor1"] = 0,
        ["Deferral_Floor2"] = 0,
        ["Deferral_Floor3"] = 200,
        ["Deferral_Floor4"] = 200,
        ["Fountain_Floor1"] = 0,
        ["Fountain_Floor2"] = 0,
        ["Fountain_Floor3"] = 0,
        ["Fountain_Floor4"] = 0,
        ["Merchant_Floor1"] = 551,
        ["Merchant_Floor2"] = 551,
        ["Merchant_Floor3"] = 3,
        ["Merchant_Floor4"] = 3,
        ["RainbowFountain_Floor1"] = 2400,
        ["RainbowFountain_Floor2"] = 2400,
        ["RainbowFountain_Floor3"] = 2300,
        ["RainbowFountain_Floor4"] = 2300,
        ["Treasure_Floor1"] = 550,
        ["Treasure_Floor2"] = 550,
        ["Treasure_Floor3"] = 2,
        ["Treasure_Floor4"] = 2,
        ["TreasureMinor_Floor1"] = 300,
        ["TreasureMinor_Floor2"] = 300,
        ["TreasureMinor_Floor3"] = 1,
        ["TreasureMinor_Floor4"] = 1,

        // Doesn't Matter
        ["Final"] = 99999,
    };

    public Dictionary<string, int> AfflictionWeights = new()
    {
        // Must be above 10k to prevent 1 Divine, 20k to prevent 2 Divine
        // Must be above 2305 to prevent 16 sextants
        // Major
        ["Anomaly Attractor"] = -100,
        ["Chiselled Stone"] = -437,
        ["Corrosive Concoction"] = 0,
        ["Cutpurse"] = -865,
        ["Deadly Snare"] = -10000,
        ["Death Toll"] = -2306,
        ["Demonic Skull"] = -2306,
        ["Ghastly Scythe"] = -9999,
        ["Glass Shard"] = -865,
        ["Orb of Negation"] = -437,
        ["Unassuming Brick"] = -865,
        ["Veiled Sight"] = -2306,

        ["Accursed Prism"] = -5765,
        ["Black Smoke"] = -2306,
        ["Blunt Sword"] = -4612,
        ["Chains of Binding"] = 0,
        ["Charred Coin"] = -437,
        ["Concealed Anomaly"] = -100,
        ["Corrupted Lockpick"] = -383,
        ["Dark Pit"] = -2306,
        ["Deceptive Mirror"] = -6918,
        ["Door Tax"] = -437,
        ["Empty Trove"] = -437,
        ["Fiendish Wings"] = -437,
        ["Floor Tax"] = -437,
        ["Gargoyle Totem"] = 0,
        ["Golden Smoke"] = -6918,
        ["Haemorrhage"] = 0,
        ["Honed Claws"] = 0,
        ["Hungry Fangs"] = 0,
        ["Iron Manacles"] = 0,
        ["Liquid Cowardice"] = -1276,
        ["Mark of Terror"] = 0,
        ["Phantom Illusion"] = 0,
        ["Poisoned Water"] = 0,
        ["Purple Smoke"] = -612,
        ["Rapid Quicksand"] = -437,
        ["Red Smoke"] = -6918,
        ["Rusted Coin"] = -437,
        ["Rusted Mallet"] = 0,
        ["Sharpened Arrowhead"] = 0,
        ["Shattered Shield"] = 0,
        ["Spiked Exit"] = -638,
        ["Spiked Shell"] = -4612,
        ["Spilt Purse"] = 0,
        ["Tattered Blindfold"] = 0,
        ["Tight Choker"] = -383,
        ["Unhallowed Amulet"] = 0,
        ["Unhallowed Ring"] = 0,
        ["Unholy Urn"] = -383,
        ["Unquenched Thirst"] = 0,
        ["Voodoo Doll"] = 0,
        ["Weakened Flesh"] = -1224,
        ["Worn Sandals"] = -1224,
    };

}
