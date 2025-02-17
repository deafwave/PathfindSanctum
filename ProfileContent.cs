using System.Collections.Generic;

namespace PathfindSanctum;

public class ProfileContent
{
    public Dictionary<string, int> CurrencyWeights { get; set; }
    public Dictionary<string, int> FightRoomWeights { get; set; }
    public Dictionary<string, int> RewardRoomWeights { get; set; }
    public Dictionary<string, int> AfflictionWeights { get; set; }

    private static ProfileContent CreateBaseProfile()
    {
        return new ProfileContent
        {
            CurrencyWeights = new()
            {
                ["Armourer's Scraps_Now"] = 1,
                ["Armourer's Scraps_EndOfFloor"] = 1,
                ["Armourer's Scraps_EndOfSanctum"] = 1,
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
                ["Chaos Orbs_EndOfSanctum"] = 611, // 14 chaos orbs
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
                ["Exalted Orbs_Now"] = 305,
                ["Exalted Orbs_EndOfFloor"] = 305,
                ["Exalted Orbs_EndOfSanctum"] = 610,
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
                ["Stacked Decks_EndOfSanctum"] = 1310, // 12 stacked decks (24c)
                ["Vaal Orbs_Now"] = 134,
                ["Vaal Orbs_EndOfFloor"] = 268,
                ["Vaal Orbs_EndOfSanctum"] = 402,
                ["Veiled Orbs_Now"] = 344,
                ["Veiled Orbs_EndOfFloor"] = 344,
                ["Veiled Orbs_EndOfSanctum"] = 689,
            },
            FightRoomWeights = new()
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
            },
            RewardRoomWeights = new()
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
            },
            AfflictionWeights = new()
            {
                // Must be above 10k to prevent 1 Divine, 20k to prevent 2 Divine
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
                ["Dark Pit"] = -6918,
                ["Deceptive Mirror"] = -6918,
                ["Door Tax"] = -437,
                ["Empty Trove"] = -437,
                ["Fiendish Wings"] = -437,
                ["Floor Tax"] = -437,
                ["Gargoyle Totem"] = 0,
                ["Golden Smoke"] = -865,
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
                ["Weakened Flesh"] = -612,
                ["Worn Sandals"] = -1224,
            }
        };
    }

    public static ProfileContent CreateDefaultProfile()
    {
        var profile = CreateBaseProfile();

        return profile;
    }

    public static ProfileContent CreateNoHitProfile()
    {
        var profile = CreateBaseProfile();
        profile.AfflictionWeights["Weakened Flesh"] = 0;
        profile.AfflictionWeights["Fiendish Wings"] = -1224;

        // TODO: Overrides for no-hit runs
        return profile;
    }

    public static ProfileContent CreatePhreciaProfile()
    {
        var profile = CreateNoHitProfile();

        profile.CurrencyWeights["Orbs of Unmaking_Now"] = 0;
        profile.CurrencyWeights["Orbs of Unmaking_EndOfFloor"] = 0;
        profile.CurrencyWeights["Orbs of Unmaking_EndOfSanctum"] = 0;

        return profile;
    }
}