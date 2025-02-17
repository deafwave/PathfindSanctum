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
                ["Armourer's Scraps_Now"] = 4,
                ["Armourer's Scraps_EndOfFloor"] = 4,
                ["Armourer's Scraps_EndOfSanctum"] = 4,
                ["Awakened Sextants_Now"] = 261,
                ["Awakened Sextants_EndOfFloor"] = 261,
                ["Awakened Sextants_EndOfSanctum"] = 698,
                ["Blacksmith's Whetstones_Now"] = 3,
                ["Blacksmith's Whetstones_EndOfFloor"] = 3,
                ["Blacksmith's Whetstones_EndOfSanctum"] = 3,
                ["Blessed Orbs_Now"] = 45,
                ["Blessed Orbs_EndOfFloor"] = 45,
                ["Blessed Orbs_EndOfSanctum"] = 68,
                ["Cartographer's Chisels_Now"] = 198,
                ["Cartographer's Chisels_EndOfFloor"] = 397,
                ["Cartographer's Chisels_EndOfSanctum"] = 556,
                ["Chaos Orbs_Now"] = 218,
                ["Chaos Orbs_EndOfFloor"] = 436,
                ["Chaos Orbs_EndOfSanctum"] = 611,
                ["Chromatic Orbs_Now"] = 39,
                ["Chromatic Orbs_EndOfFloor"] = 87,
                ["Chromatic Orbs_EndOfSanctum"] = 130,
                ["Divine Orbs_Now"] = 10000,
                ["Divine Orbs_EndOfFloor"] = 10000,
                ["Divine Orbs_EndOfSanctum"] = 20000,
                ["Divine Vessels_Now"] = 21,
                ["Divine Vessels_EndOfFloor"] = 21,
                ["Divine Vessels_EndOfSanctum"] = 65,
                ["Enkindling Orbs_Now"] = 139,
                ["Enkindling Orbs_EndOfFloor"] = 139,
                ["Enkindling Orbs_EndOfSanctum"] = 209,
                ["Exalted Orbs_Now"] = 349,
                ["Exalted Orbs_EndOfFloor"] = 349,
                ["Exalted Orbs_EndOfSanctum"] = 698,
                ["Gemcutter's Prisms_Now"] = 209,
                ["Gemcutter's Prisms_EndOfFloor"] = 418,
                ["Gemcutter's Prisms_EndOfSanctum"] = 628,
                ["Glassblower's Baubles_Now"] = 157,
                ["Glassblower's Baubles_EndOfFloor"] = 349,
                ["Glassblower's Baubles_EndOfSanctum"] = 523,
                ["Instilling Orbs_Now"] = 69,
                ["Instilling Orbs_EndOfFloor"] = 139,
                ["Instilling Orbs_EndOfSanctum"] = 209,
                ["Jeweller's Orbs_Now"] = 15,
                ["Jeweller's Orbs_EndOfFloor"] = 34,
                ["Jeweller's Orbs_EndOfSanctum"] = 52,
                ["Mirrors of Kalandra_Now"] = 1000000,
                ["Mirrors of Kalandra_EndOfFloor"] = 1000000,
                ["Mirrors of Kalandra_EndOfSanctum"] = 1000000,
                ["Orbs of Alchemy_Now"] = 10,
                ["Orbs of Alchemy_EndOfFloor"] = 24,
                ["Orbs of Alchemy_EndOfSanctum"] = 34,
                ["Orbs of Alteration_Now"] = 74,
                ["Orbs of Alteration_EndOfFloor"] = 165,
                ["Orbs of Alteration_EndOfSanctum"] = 248,
                ["Orbs of Annulment_Now"] = 187,
                ["Orbs of Annulment_EndOfFloor"] = 187,
                ["Orbs of Annulment_EndOfSanctum"] = 375,
                ["Orbs of Augmentation_Now"] = 1,
                ["Orbs of Augmentation_EndOfFloor"] = 1,
                ["Orbs of Augmentation_EndOfSanctum"] = 1,
                ["Orbs of Binding_Now"] = 6,
                ["Orbs of Binding_EndOfFloor"] = 13,
                ["Orbs of Binding_EndOfSanctum"] = 18,
                ["Orbs of Chance_Now"] = 15,
                ["Orbs of Chance_EndOfFloor"] = 34,
                ["Orbs of Chance_EndOfSanctum"] = 52,
                ["Orbs of Fusing_Now"] = 39,
                ["Orbs of Fusing_EndOfFloor"] = 91,
                ["Orbs of Fusing_EndOfSanctum"] = 130,
                ["Orbs of Horizon_Now"] = 22,
                ["Orbs of Horizon_EndOfFloor"] = 45,
                ["Orbs of Horizon_EndOfSanctum"] = 68,
                ["Orbs of Regret_Now"] = 174,
                ["Orbs of Regret_EndOfFloor"] = 349,
                ["Orbs of Regret_EndOfSanctum"] = 488,
                ["Orbs of Scouring_Now"] = 54,
                ["Orbs of Scouring_EndOfFloor"] = 109,
                ["Orbs of Scouring_EndOfSanctum"] = 152,
                ["Orbs of Transmutation_Now"] = 0,
                ["Orbs of Transmutation_EndOfFloor"] = 0,
                ["Orbs of Transmutation_EndOfSanctum"] = 0,
                ["Orbs of Unmaking_Now"] = 174,
                ["Orbs of Unmaking_EndOfFloor"] = 349,
                ["Orbs of Unmaking_EndOfSanctum"] = 523,
                ["Regal Orbs_Now"] = 69,
                ["Regal Orbs_EndOfFloor"] = 139,
                ["Regal Orbs_EndOfSanctum"] = 209,
                ["Sacred Orbs_Now"] = 654,
                ["Sacred Orbs_EndOfFloor"] = 654,
                ["Sacred Orbs_EndOfSanctum"] = 1309,
                ["Stacked Decks_Now"] = 349,
                ["Stacked Decks_EndOfFloor"] = 698,
                ["Stacked Decks_EndOfSanctum"] = 1047,
                ["Vaal Orbs_Now"] = 111,
                ["Vaal Orbs_EndOfFloor"] = 223,
                ["Vaal Orbs_EndOfSanctum"] = 335,
                ["Veiled Orbs_Now"] = 74192,
                ["Veiled Orbs_EndOfFloor"] = 74192,
                ["Veiled Orbs_EndOfSanctum"] = 148385,
                ["Veiled Scarabs_Now"] = 261,
                ["Veiled Scarabs_EndOfFloor"] = 261,
                ["Veiled Scarabs_EndOfSanctum"] = 698,
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