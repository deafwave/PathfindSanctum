using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Elements.Sanctum;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Enums;
using GameOffsets.Native;

namespace PathfindSanctum;

public class RewardHelper(
    PluginBridge pluginBridge,
    IngameState gameState,
    ExileCore.Graphics graphics,
    PathFinder pathFinder,
    SanctumStateTracker stateTracker
)
{
    /** Meant to be used for day 1-2 when NinjaPricer is not working */
    private readonly Dictionary<string, double> baseCurrencyValues = new()
    {
        {"Orb of Transmutation", 0.01},
        {"Orb of Augmentation", 0.03},
        {"Orb of Binding", 0.03},
        {"Orb of Alchemy", 0.04},
        {"Orb of Chance", 0.04},
        {"Jeweller's Orb", 0.04},
        {"Blacksmith's Whetstone", 0.07},
        {"Chromatic Orb", 0.1},
        {"Armourer's Scrap", 0.11},
        {"Orb of Horizons", 0.13},
        {"Blessed Orb", 0.13},
        {"Orb of Fusing", 0.15},
        {"Orb of Alteration", 0.19},
        {"Orb of Scouring", 0.25},
        {"Instilling Orb", 0.4},
        {"Enkindling Orb", 0.4},
        {"Glassblower's Bauble", 0.4},
        {"Regal Orb", 0.4},
        {"Divine Vessel", 0.5},
        {"Vaal Orb", 0.64},
        {"Cartographer's Chisel", 0.91},
        {"Orb of Regret", 0.8},
        {"Orb of Unmaking", 0}, // 0 for phrecia, 1 for not
        {"Chaos Orb", 1},
        {"Gemcutter's Prism", 1.2},
        {"Orb of Annulment", 4.3},
        {"Veiled Orb", 1700},
        {"Stacked Deck", 2},
        {"Exalted Orb", 8},
        {"Sacred Orb", 15},
        {"Divine Orb", 170.5},
        {"Mirror of Kalandra", 128600},
    };

    bool usingDivinity = false;
    SanctumFloorWindow floorWindow;
    SanctumRewardWindow sanctumRewardWindow;
    int floor;

    public void DrawRewards()
    {
        usingDivinity = gameState.Data.MapStats.Any(x => x.Key.ToString() == "MapLycia2DuplicateUpToXDeferredRewards");
        floor = stateTracker.PlayerFloor;
        floorWindow = gameState.IngameUi.SanctumFloorWindow;
        sanctumRewardWindow = gameState.IngameUi.SanctumRewardWindow;
        if (floorWindow == null || !sanctumRewardWindow.IsVisible || sanctumRewardWindow.RewardElements.Count == 0 || sanctumRewardWindow.RewardElements[0].ChildCount >= 3)
            return;
        
        var rewardValues = GetRewardValues(sanctumRewardWindow.RewardElements);
        DrawDivinityText(usingDivinity, sanctumRewardWindow.PositionNum);

        if (usingDivinity)
        {
            DrawRewardsDivinity(rewardValues);
        }
        else
        {
            DrawRewardsSimple(rewardValues);
        }
    }

    private class Reward
    {
        public string Name;
        public int Count;
        public double Value;
    }

    private void DrawDivinityText(bool usingDivinity, Vector2 position)
    {
        string text = usingDivinity ? "Divinity Detected" : "No Divinity Detected";
        graphics.DrawText(text, position - new Vector2(100, 40), SharpDX.Color.White, 45, FontAlign.Center);
    }

    private void DrawRewardElements(Dictionary<Element, Reward> rewardValues, KeyValuePair<Element, Reward> bestReward)
    {
        foreach (var reward in rewardValues)
        {
            var rewardPos = reward.Key.Children[1].GetClientRectCache;

            if (reward.Key.Address == bestReward.Key.Address)
            {
                DrawBestReward(reward, rewardPos);
            }
            else if (reward.Key.Address != sanctumRewardWindow.RewardElements.Last().Address || floor == 4 || !usingDivinity)
            {
                DrawNonBestReward(reward, rewardPos);
            }
            else
            {
                DrawDivinityWarning(reward, rewardPos);
            }
        }
    }

    private void DrawBestReward(KeyValuePair<Element, Reward> reward, SharpDX.RectangleF rewardPos)
    {
        // Different color for divines & mirrors
        SharpDX.Color boxColor = reward.Value.Name.Contains("Divine Orb") || reward.Value.Name.Contains("Mirror") ? SharpDX.Color.Magenta : SharpDX.Color.DarkGreen;
        string text = reward.Value.Name.Contains("Divine Orb") ? "TAKE THE DIVINES..." : "TAKE THIS";

        graphics.DrawBox(rewardPos, boxColor);
        graphics.DrawText(text, new Vector2(rewardPos.Center.X, rewardPos.Center.Y) - new Vector2(0, 7), SharpDX.Color.White, 45, FontAlign.Center);
        graphics.DrawText($"{reward.Value.Count}x {reward.Value.Name} ({reward.Value.Value})", new Vector2(rewardPos.Center.X, rewardPos.Center.Y) + new Vector2(0, 8), SharpDX.Color.White, 45, FontAlign.Center);
    }

    private void DrawNonBestReward(KeyValuePair<Element, Reward> reward, SharpDX.RectangleF rewardPos)
    {
        graphics.DrawBox(rewardPos, SharpDX.Color.DarkRed);
        graphics.DrawText("DONT TAKE", new Vector2(rewardPos.Center.X, rewardPos.Center.Y) - new Vector2(0, 7), SharpDX.Color.White, 45, FontAlign.Center);
        graphics.DrawText($"{reward.Value.Count}x {reward.Value.Name}", new Vector2(rewardPos.Center.X, rewardPos.Center.Y) + new Vector2(0, 8), SharpDX.Color.White, 45, FontAlign.Center);
    }

    private void DrawDivinityWarning(KeyValuePair<Element, Reward> reward, SharpDX.RectangleF rewardPos)
    {
        graphics.DrawBox(rewardPos, SharpDX.Color.DarkRed);
        graphics.DrawText($"THIS IS FLOOR {floor}...", new Vector2(rewardPos.Center.X, rewardPos.Center.Y) - new Vector2(0, 10), SharpDX.Color.White, 45, FontAlign.Center);
        graphics.DrawText("DONT TAKE LAST REWARD", new Vector2(rewardPos.Center.X, rewardPos.Center.Y) + new Vector2(0, 5), SharpDX.Color.White, 45, FontAlign.Center);
        graphics.DrawText($"{reward.Value.Count}x {reward.Value.Name}", new Vector2(rewardPos.Center.X, rewardPos.Center.Y) + new Vector2(0, 20), SharpDX.Color.White, 45, FontAlign.Center);
    }

    private void DrawFloor4InfoText(Vector2 position, int pactCounter, int rewardsWeCanTake, int divCounter)
    {
        graphics.DrawText($"Divines Remaining: {divCounter}", position - new Vector2(160, 0), SharpDX.Color.White, 25, FontAlign.Left);
        graphics.DrawText($"Pacts Remaining: {pactCounter}", position - new Vector2(160, -20), SharpDX.Color.White, 25, FontAlign.Left);
        graphics.DrawText($"Rewards We Can Take: {rewardsWeCanTake}", position - new Vector2(160, -40), SharpDX.Color.White, 25, FontAlign.Left);
    }

    private void DrawRewardsForFloor4(Dictionary<Element, Reward> rewardValues)
    {
        var (divCounter, pactCounter, rewardsWeCanTake) = CalculateFloor4Metrics();

        DrawFloor4InfoText(sanctumRewardWindow.PositionNum, pactCounter, rewardsWeCanTake, divCounter);

        if (divCounter <= 1 && rewardsWeCanTake >= 1 && !IsInPactRoom())
        {
            DrawSelectedRewardsForFloor4(rewardValues, rewardsWeCanTake);
        }
        else
        {
            // Idk what this one does
            if (rewardValues.Any(x => x.Value.Name.Contains("Divine Orb") || x.Value.Name.Contains("Mirror")) || rewardsWeCanTake >= 1 && pactCounter == 0)
            {
                DrawRewardElements(rewardValues, rewardValues.OrderByDescending(x => x.Value.Value).FirstOrDefault());
            }
            else
            {
                DrawRewardElements(rewardValues, rewardValues.FirstOrDefault());
            }
        }
    }

    private void DrawSelectedRewardsForFloor4(Dictionary<Element, Reward> rewardValues, int rewardsWeCanTake)
    {
        var selectedRewards = rewardValues.OrderByDescending(x => x.Value.Value)
                                          .Take(rewardsWeCanTake);
        if (selectedRewards.Count() == 0)
        {
            DrawRewardElements(rewardValues, rewardValues.FirstOrDefault());
            return;
        }

        DrawRewardElements(rewardValues, selectedRewards.FirstOrDefault());
    }

    private void DrawRewardsSimple(Dictionary<Element, Reward> rewardValues)
    {
        var bestReward = rewardValues.OrderByDescending(x => x.Value.Value).FirstOrDefault();
        DrawRewardElements(rewardValues, bestReward);
    }

    private void DrawRewardsDivinity(Dictionary<Element, Reward> rewardValues)
    {
        if (floor <= 2)
        {
            var bestReward = GetBestRewardSkipLast(rewardValues, sanctumRewardWindow.RewardElements.Last().Address);
            DrawRewardElements(rewardValues, bestReward);
        }
        else if (floor == 3)
        {
            var bestReward = DetermineBestRewardForFloor3(rewardValues);
            DrawRewardElements(rewardValues, bestReward);
        }
        else if (floor == 4)
        {
            DrawRewardsForFloor4(rewardValues);
        }
    }

    private static KeyValuePair<Element, Reward> GetBestRewardSkipLast(Dictionary<Element, Reward> rewardValues, long lastRewardAddress)
    {
        return rewardValues.Where(x => x.Key.Address != lastRewardAddress)
                            .OrderByDescending(x => x.Value.Value).FirstOrDefault();
    }

    private bool IsInPactRoom()
    {
        var currentRoom = stateTracker.GetCurrentRoom();
        return currentRoom.RewardType.Contains("Deal");
    }



    private Dictionary<Element, Reward> GetRewardValues(IEnumerable<Element> rewardElements) {
        
    var rewardValues = new Dictionary<Element, Reward>();
    var regex = new Regex(@"(Receive)\s((?'rewardcount'(\d+))x(?'rewardname'.*))\s(right now|at the end of the next Floor|at the end of the Floor|on completing the Sanctum)$");

    foreach (var reward in rewardElements)
    {
        var match = regex.Match(reward.Children[1].Text);
        if (!match.Success) continue;

        var rewardName = match.Groups["rewardname"].Value.Trim();
        if (!int.TryParse(match.Groups["rewardcount"].ValueSpan.Trim(), out var stackSize)) continue;

        var baseName = rewardName.Replace("Orbs", "Orb").Replace("Mirrors", "Mirror").TrimEnd('s');
        if (rewardName.Equals("Orb of Horizon", StringComparison.OrdinalIgnoreCase))
        {
            baseName = "Orb of Horizons";
        }

        var data = new BaseItemType
        {
            BaseName = baseName,
            ClassName = "StackableCurrency",
            Metadata = ""
        };

        var fn = pluginBridge.GetMethod<Func<BaseItemType, double>>("NinjaPrice.GetBaseItemTypeValue");
        var value = (fn != null ? fn(data) : baseCurrencyValues.GetValueOrDefault(data.BaseName, 0)) * stackSize;

        rewardValues.Add(reward, new Reward { Name = rewardName, Count = stackSize, Value = value });
    }

    return rewardValues;
}

    private KeyValuePair<Element, Reward> DetermineBestRewardForFloor3(Dictionary<Element, Reward> rewardValues) {
    bool hasEightRoomChoices = floorWindow.FloorData.RoomChoices.Count == 8;
    bool containsSpecialRewards = rewardValues.Any(x => x.Value.Name.Contains("Divine Orb") || x.Value.Name.Contains("Mirror"));

    if (hasEightRoomChoices)
    {
        if (!containsSpecialRewards)
        {
            return rewardValues
                .Where(x => x.Key.Address == sanctumRewardWindow.RewardElements.First().Address)
                .OrderByDescending(x => x.Value.Value)
                .FirstOrDefault();
        }
        else
        {
            return rewardValues
                .OrderByDescending(x => x.Value.Value)
                .FirstOrDefault();
        }
    }

    return rewardValues
        .Where(x => x.Key.Address != sanctumRewardWindow.RewardElements.Last().Address || containsSpecialRewards)
        .OrderByDescending(x => x.Value.Value)
        .FirstOrDefault();
}
    private (int divCounter, int pactCounter, int rewardsWeCanTake) CalculateFloor4Metrics()
    {
        int divCounter = 0;
        int pactCounter = 0;

        List<(int, int)> path = [];
        pathFinder.CreateRoomWeightMap();
        path = pathFinder.FindBestPath();

        foreach (var room in path)
        {
            if (IsCurrentPlayerRoom(room)) continue;

            var sanctumRoom = floorWindow.RoomsByLayer[room.Item1][room.Item2];

            if (sanctumRoom?.Data?.RewardRoom?.Id.Contains("_Deal") ?? false)
            {
                pactCounter++;
            }

            // Idk what this one does
            if (new[] { sanctumRoom?.Data?.Reward1, sanctumRoom?.Data?.Reward2, sanctumRoom?.Data?.Reward3 }.Any(x => x != null && (x.CurrencyName.Contains("Divine Orb") || x.CurrencyName.Contains("Mirror"))))
            {
                divCounter++;
            }
        }

        var floorData = RemoteMemoryObject.GetObjectStatic<SanctumFloorData>(floorWindow.FloorData.Address - 0x88);
        var rewards = floorData.M.ReadStdVectorStride<long>(floorData.M.Read<StdVector>(floorData.Address + 0x70), 0x10)
            .Select(TheGame.pTheGame.Files.SanctumDeferredRewards.GetByAddressOrReload)
            .Where(x => x != null).ToList();


        int currentRewardCount = rewards.Count(x => x.DeferralCategory.Id.Contains("FinalBoss") || x.DeferralCategory.Id.Contains("RewardFloor"));

        int rewardsWeCanTake = 2 - currentRewardCount - pactCounter - divCounter;

        return (divCounter, pactCounter, rewardsWeCanTake);
    }

    private bool IsCurrentPlayerRoom((int, int) room)
    {
        return room.Item1 == stateTracker.PlayerLayerIndex && room.Item2 == stateTracker.PlayerRoomIndex;
    }

}
