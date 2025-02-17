using ExileCore;
using System.Linq;
using ExileCore.PoEMemory;

namespace PathfindSanctum;

public class PathfindSanctumPlugin : BaseSettingsPlugin<PathfindSanctumSettings>
{
    private readonly SanctumStateTracker stateTracker = new();
    private PathFinder pathFinder;
    private WeightCalculator weightCalculator;
    private RewardHelper rewardHelper;
    private EffectHelper effectHelper;

    public override bool Initialise()
    {
        weightCalculator = new WeightCalculator(GameController, Settings);
        pathFinder = new PathFinder(Graphics, Settings, stateTracker, weightCalculator);
        rewardHelper = new RewardHelper(GameController, GameController.PluginBridge, GameController.IngameState, Graphics, pathFinder, stateTracker);
        effectHelper = new EffectHelper(GameController, Graphics);
        return base.Initialise();
    }

    // Fixes character select file reload issue
    // Repro: Menagerie -> Hideout -> Character Select -> Character in LA -> Character Select -> Sanctum -> enter -> no affliction data
    public override void AreaChange(AreaInstance area)
    {
        // Might help with refreshing files so data exists.
        var files = new FilesContainer(GameController.Game.M);
        var isDiff = false;
        if (files.SanctumRooms.EntriesList.Count != GameController.Files.SanctumRooms.EntriesList.Count)
        {
            isDiff = true;
        }
        else
        {
            if (files.SanctumRooms.EntriesList.Where((t, i) => t.Address != GameController.Files.SanctumRooms.EntriesList[i].Address).Any())
            {
                isDiff = true;
            }
        }

        if (isDiff)
        {
            GameController.Game.ReloadFiles();
        }
    }

    public override void Render()
    {
        if (!GameController.Game.IngameState.InGame)
            return;

        if (
            !GameController.Area.CurrentArea.Area.RawName.StartsWith("Sanctum", System.StringComparison.OrdinalIgnoreCase)
        )
            return;

        rewardHelper.DrawRewards();
        effectHelper.DrawEffects();

        var floorWindow = GameController.Game.IngameState.IngameUi.SanctumFloorWindow;
        if (floorWindow == null || !floorWindow.IsVisible)
            return;

        // if (!GameController.Files.SanctumRooms.EntriesList.Any() && _sinceLastReloadStopwatch.Elapsed > TimeSpan.FromSeconds(5))
        // {
        //    GameController.Files.LoadFiles();
        //    _sinceLastReloadStopwatch.Restart();
        // }

        if (
            stateTracker.HasRoomData()
            && !stateTracker.IsSameSanctum(GameController.Area.CurrentArea)
        )
        {
            stateTracker.Reset(GameController.Area.CurrentArea);
            return;
        }


        stateTracker.UpdateRoomStates(floorWindow);
        UpdateAndRenderPath();
    }

    private void UpdateAndRenderPath()
    {
        // TODO: Optimize this so it's not executed on every render (maybe only executed if we updated our known states)
        pathFinder.CreateRoomWeightMap();

        pathFinder.DrawInfo();

        pathFinder.FindBestPath();
        pathFinder.DrawBestPath();
    }

}
