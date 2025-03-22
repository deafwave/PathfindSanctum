using ExileCore;
using System;
using System.Diagnostics;

namespace PathfindSanctum;

public class PathfindSanctumPlugin : BaseSettingsPlugin<PathfindSanctumSettings>
{
    private readonly SanctumStateTracker stateTracker = new();
    private PathFinder pathFinder;
    private WeightCalculator weightCalculator;
    private RewardHelper rewardHelper;
    private EffectHelper effectHelper;
    private readonly Stopwatch _sinceLastPathfindStopwatch = Stopwatch.StartNew();

    public override bool Initialise()
    {
        weightCalculator = new WeightCalculator(GameController, Settings);
        pathFinder = new PathFinder(Graphics, Settings, stateTracker, weightCalculator);
        rewardHelper = new RewardHelper(GameController.PluginBridge, GameController.IngameState, Graphics, pathFinder, stateTracker);
        effectHelper = new EffectHelper(GameController, Graphics);
        return base.Initialise();
    }

    // NOTE: Minor bug, but data fixes itself after a few seconds (e.g. 1 room)
    // Repro: Menagerie -> Hideout -> Character Select -> Character in LA -> Character Select -> Sanctum -> enter -> no affliction/reward data

    public override void Render()
    {
        if (!GameController.Game.IngameState.InGame)
            return;

        if (
            !GameController.Area.CurrentArea.Area.Id.StartsWith("Sanctum", System.StringComparison.OrdinalIgnoreCase)
        )
            return;

        rewardHelper.DrawRewards();
        effectHelper.DrawEffects();

        var floorWindow = GameController.Game.IngameState.IngameUi.SanctumFloorWindow;
        if (floorWindow == null || !floorWindow.IsVisible)
            return;

        if (
            stateTracker.HasRoomData()
            && !stateTracker.IsSameSanctum(GameController.Area.CurrentArea)
        )
        {
            stateTracker.Reset(GameController.Area.CurrentArea);
            UpdateAndRenderPath();
            return;
        }

        UpdateAndRenderPath();
    }

    private void UpdateAndRenderPath(bool forceUpdate = false)
    {
        // TODO: Find a better way to optimize this so it's not executed on every render
        // E.g. Only executed if we updated our room states
        stateTracker.UpdateRoomStates(GameController.Game.IngameState.IngameUi.SanctumFloorWindow); // FIXME: Why does this need to be called every render?
        if (forceUpdate || _sinceLastPathfindStopwatch.Elapsed > TimeSpan.FromSeconds(5))
        {
            pathFinder.CreateRoomWeightMap();
            pathFinder.FindBestPath();
            _sinceLastPathfindStopwatch.Restart();
        }

        pathFinder.DrawInfo();
        pathFinder.DrawBestPath();
    }

}
