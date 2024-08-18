using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using SharpDX;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace BetterSanctum;

public class BetterSanctumPlugin : BaseSettingsPlugin<BetterSanctumSettings>
{
    private readonly Stopwatch _sinceLastReloadStopwatch = Stopwatch.StartNew();

    private bool dataCalculated;
    private List<(int, int)> bestPath;

    private bool IsEntityWithinScreen(Vector2 entityPos, RectangleF screensize, float allowancePX)
    {
        // Check if the entity position is within the screen bounds with allowance
        float leftBound = screensize.Left - allowancePX;
        float rightBound = screensize.Right + allowancePX;
        float topBound = screensize.Top - allowancePX;
        float bottomBound = screensize.Bottom + allowancePX;

        return entityPos.X >= leftBound && entityPos.X <= rightBound &&
               entityPos.Y >= topBound && entityPos.Y <= bottomBound;
    }

    public override void Render()
    {
        var entityList = GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Effect]
            .Where(x => x.Metadata.Contains("/Effects/Effect") &&
                        x.TryGetComponent<Animated>(out var animComp) &&
                        animComp?.BaseAnimatedObjectEntity.Metadata != null) ?? [];

        foreach (var entity in entityList)
        {
            var animComp = entity.GetComponent<Animated>();
            var metadata = animComp.BaseAnimatedObjectEntity.Metadata;
            var pos = RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen(entity.PosNum);

            if (metadata.Contains("League_Sanctum/hazards/hazard_meteor"))
            {
                DrawHazard("Meteor", pos, entity.PosNum, 140.0f);
            }
            else if (metadata.Contains("League_Sanctum/hazards/totem_holy_beam_impact"))
            {
                DrawHazard("ZAP!", pos, entity.PosNum, 40.0f);
            }
            else if (metadata.Contains("League_Necropolis/LyciaBoss/ao/lightning_strike_scourge"))
            {
                if (entity.TryGetComponent<AnimationController>(out var animController) &&
                    animController.AnimationProgress is > 0.0f and < 0.3f)
                {
                    DrawHazard("Dodge", pos, entity.PosNum, 100.0f, 60);
                }
            }
        }

        void DrawHazard(string text, Vector2 screenPos, Vector3 worldPos, float radius, int segments = 12)
        {
            Graphics.DrawTextWithBackground(text, screenPos, Color.Red, FontAlign.Center, Color.Black);
            Graphics.DrawFilledCircleInWorld(worldPos, radius, Color.Red with { A = 150 }, segments);
        }

        var floorWindow = GameController.IngameState.IngameUi.SanctumFloorWindow;
        if (!floorWindow.IsVisible)
        {
            dataCalculated = false;
            return;
        }

        if (!GameController.Files.SanctumRooms.EntriesList.Any() && _sinceLastReloadStopwatch.Elapsed > TimeSpan.FromSeconds(5))
        {
            GameController.Files.LoadFiles();
            _sinceLastReloadStopwatch.Restart();
        }

        var roomsByLayer = floorWindow.RoomsByLayer;
        var pathFinder = new PathFinder(this);

        pathFinder.CreateRoomWeightMap();

        if (!this.dataCalculated)
        {
            dataCalculated = true;
            pathFinder.CreateRoomWeightMap();
            this.bestPath = pathFinder.FindBestPath();
        }
        else
        {
            foreach (var room in this.bestPath)
            {
                if (room.Item1 == pathFinder.playerLayerIndex && room.Item2 == pathFinder.playerRoomIndex) continue;

                var sanctumRoom = roomsByLayer[room.Item1][room.Item2];
                Graphics.DrawFrame(sanctumRoom.GetClientRectCache, Settings.BestPathColor, Settings.FrameThickness);
            }
        }
    }
}
