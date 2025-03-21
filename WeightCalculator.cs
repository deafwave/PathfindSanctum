using System;
using System.Linq;
using System.Text;
using ExileCore;
using ExileCore.Shared.Enums;

namespace PathfindSanctum;

public class WeightCalculator(GameController gameController, PathfindSanctumSettings settings)
{
    private readonly GameController gameController = gameController;
    private readonly PathfindSanctumSettings settings = settings;
    private readonly StringBuilder displayText = new();
    private readonly StringBuilder debugText = new();

    private static readonly string[] majorAfflictions = { "Anomaly Attractor", "Chiselled Stone", "Corrosive Concoction", "Cutpurse", "Deadly Snare", "Death Toll", "Demonic Skull", "Ghastly Scythe", "Glass Shard", "Orb of Negation", "Unassuming Brick", "Veiled Sight" };
    private string floorSuffix = $"_Floor1";
    private int floorNumber = 1;
    private bool randomAfflictionOnAffliction = false;
    private bool ignoreMinorAfflictions = false;
    private bool trapResolveAffliction = false;

    private int currentResolve = 0;
    private int inspiration = 0;
    private int gold = 0;
    private int maxResolve = 0;

    public (double weight, string debug, string display) CalculateRoomWeight(RoomState room, SanctumStateTracker stateTracker)
    {
        if (room == null)
            return (0, string.Empty, string.Empty);

        debugText.Clear();
        displayText.Clear();
        var profile = settings.GetCurrentProfile();
        double weight = 1000000;

        // TODO: Optimize this
        floorNumber = stateTracker.PlayerFloor;
        currentResolve = stateTracker.PlayerResolve;
        inspiration = stateTracker.PlayerInspiration;
        gold = stateTracker.PlayerGold;
        maxResolve = stateTracker.PlayerMaxResolve;
        floorSuffix = $"_Floor{floorNumber}";
        var mapStats = gameController.IngameState.Data.MapStats;
        randomAfflictionOnAffliction = mapStats.ContainsKey(GameStat.SanctumGainRandomMinorAfflictionOnGainingAffliction);
        ignoreMinorAfflictions = mapStats.ContainsKey(GameStat.SanctumPreventMinorAfflictions);
        trapResolveAffliction = mapStats.Any(keyValuePair => keyValuePair.Key.ToString() == "AfflictionTrapSanctumDamage");



        weight += CalculateFightTypeWeight(room, profile); // FightRoom
        weight += CalculateAfflictionWeights(room, profile); // RoomEffect
        weight += CalculateRewardWeights(room, profile); // RewardRoom
        weight += CalculateRewardTypeWeights(room, profile); // Rewards
        weight += CalculateConnectivityBonus(room);

        return (weight, debugText.ToString(), displayText.ToString());
    }

    private double CalculateFightTypeWeight(RoomState room, ProfileContent profile)
    {
        var fightType = room.FightType;
        if (fightType == null)
            return 0;

        int typeWeight = settings.GetFightRoomWeight(fightType + floorSuffix);

        if (fightType == "Arena" && trapResolveAffliction)
        {
            typeWeight *= 4;
        }
        else if (fightType == "Explore" && (currentResolve + inspiration) < 50)
        {
            typeWeight *= 10;
        }
        if (settings.DebugEnable.Value)
            debugText.AppendLine($"{fightType}:{typeWeight}");
        return typeWeight;

        //debugText.AppendLine($"Room Type ({fightType}): 0 (not found in weights)");
        //return 0;
    }

    private int CalculateAfflictionWeights(RoomState room, ProfileContent profile)
    {
        var affliction = room.Affliction;
        if (affliction == null)
            return 0;

        var afflictionName = affliction.ToString();
        int? dynamicWeight = AfflictionDynamicWeights(affliction);
        if (dynamicWeight.HasValue)
        {
            var dynamicWeightModifiedValue = AfflictionModifiers(afflictionName, dynamicWeight.Value);
            if (settings.DebugEnable.Value)
                debugText.AppendLine($"{afflictionName}:{dynamicWeightModifiedValue}");
            return dynamicWeightModifiedValue;
        }

        int staticWeight = settings.GetAfflictionWeight(affliction);
        var staticWeightModifiedValue = AfflictionModifiers(afflictionName, staticWeight);
        if (settings.DebugEnable.Value)
            debugText.AppendLine($"{afflictionName}:{staticWeightModifiedValue}");
        return staticWeightModifiedValue;
    }

    private double CalculateRewardTypeWeights(RoomState room, ProfileContent profile)
    {
        var fightType = room.RewardType;
        if (fightType == null)
            return 0;

        int typeWeight = settings.GetRewardRoomWeight(fightType + floorSuffix);
        if(fightType == "Deferral" && room.Rewards.Count > 0)
        {
            typeWeight = 0;
        }

        if (settings.DebugEnable.Value)
            debugText.AppendLine($"{fightType}:{typeWeight}");

        return typeWeight;

        //debugText.AppendLine($"Reward Type ({fightType}): 0 (not found in weights)");
        //return 0;
    }

    private double CalculateRewardWeights(RoomState room, ProfileContent profile)
    {
        // Early return if room or rewards is null
        if (room?.Rewards == null || room.Rewards.Count < 3)
        {
            return 0;
        }

        // Safely get currency names with null checks
        var rewardOne = room.Rewards[0]?.CurrencyName;
        var rewardTwo = room.Rewards[1]?.CurrencyName;
        var rewardThree = room.Rewards[2]?.CurrencyName;

        // Check if any currency names are null or empty
        if (string.IsNullOrEmpty(rewardOne) ||
            string.IsNullOrEmpty(rewardTwo) ||
            string.IsNullOrEmpty(rewardThree))
        {
            return 0;
        }

        // Calculate weights
        int rewardWeight1 = settings.GetCurrencyWeight(rewardOne + "_Now");
        int rewardWeight2 = settings.GetCurrencyWeight(rewardTwo + "_EndOfFloor");
        int rewardWeight3 = settings.GetCurrencyWeight(rewardThree + "_EndOfSanctum");

        int maxRewardWeight = Math.Max(Math.Max(rewardWeight1, rewardWeight2), rewardWeight3);

        // Append display text only for weights above threshold
        if (rewardWeight1 >= 5000)
        {
            displayText.AppendLine(rewardOne);
        }
        if (rewardWeight2 >= 5000)
        {
            displayText.AppendLine(rewardTwo);
        }
        if (rewardWeight3 >= 5000)
        {
            displayText.AppendLine(rewardThree);
        }

        // Debug logging
        if (settings.DebugEnable.Value)
        {
            debugText.AppendLine($"Currency:{maxRewardWeight}");
        }

        return maxRewardWeight;
    }

    private double CalculateConnectivityBonus(RoomState room)
    {
        var connectionBonus = room.Connections == 1 ? 0 : room.Connections == 2 ? 100 : 200;
        if (settings.DebugEnable.Value)
            debugText.AppendLine($"Connectivity:{connectionBonus}");
        return connectionBonus;
    }


    // Dynamic Afflictions
    private int AfflictionModifiers(string afflictionName, int afflictionWeight)
    {
        // Random Affliction on Affliction bonus
        if (randomAfflictionOnAffliction)
        {
            afflictionWeight += -218;
        }

        // Ignore Minor Afflictions
        if (ignoreMinorAfflictions && !majorAfflictions.Any(afflictionName.Equals))
        {
            afflictionWeight = 0;
        }

        // Floor 4 changes
        if (floorNumber == 4 && afflictionName.Equals("Floor Tax"))
        {
            afflictionWeight = 0;
        }
        return afflictionWeight;
    }
    private int? AfflictionDynamicWeights(string affliction)
    {
        return affliction switch
        {
            "Weakened Flesh" => AfflictionWeakenedFlesh(),
            _ => null,
        };
    }
    private int? AfflictionWeakenedFlesh()
    {
        if(inspiration > 100 || maxResolve > 300)
        {
            return -383;
        }

        return null;
    }
}
