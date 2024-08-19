using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Enums;
using ImGuiNET;
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
        var effectEntityList = GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Effect]
            .Where(x => x.Metadata.Contains("/Effects/Effect") &&
                        x.TryGetComponent<Animated>(out var animComp) &&
                        animComp?.BaseAnimatedObjectEntity.Metadata != null) ?? [];

        var terrainEntityList = GameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Terrain] ?? [];

        foreach (var entity in effectEntityList)
        {
            var animComp = entity.GetComponent<Animated>();
            var metadata = animComp.BaseAnimatedObjectEntity.Metadata;
            var pos = RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen(entity.PosNum);

            if (metadata.Contains("League_Sanctum/hazards/hazard_meteor"))
            {
                DrawHazard("Meteor", pos, entity.PosNum, 140.0f, 30);
            }
            else if (metadata.Contains("League_Sanctum/hazards/totem_holy_beam_impact"))
            {
                DrawHazard("ZAP!", pos, entity.PosNum, 40.0f, 30);
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

        foreach (var entity in terrainEntityList)
        {
            var pos = RemoteMemoryObject.pTheGame.IngameState.Camera.WorldToScreen(entity.PosNum);

            if (entity.Metadata.Contains("/Sanctum/Objects/Spawners/SanctumSpawner") || entity.Metadata.Contains("/Sanctum/Objects/SanctumSpawner"))
            {
                entity.TryGetComponent<StateMachine>(out var stateComponent);

                var isActive = false;
                if (stateComponent != null)
                {
                    var activeState = stateComponent.States.FirstOrDefault(x => x.Name == "active");
                    isActive = activeState is { Value: 1 };
                }

                switch (isActive)
                {
                    case true:
                        DrawHazard("Spawner", pos, entity.PosNum, 60.0f, 4, Color.Lime);
                        break;
                    case false:
                        DrawHazard(" + ", pos, entity.PosNum, 20.0f, 4, Color.LightBlue);
                        break;
                }
            }
        }

        void DrawHazard(string text, Vector2 screenPos, Vector3 worldPos, float radius, int segments, Color color = default)
        {
            if (color == default)
            {
                color = Color.Red;
            }

            var textSize = ImGui.CalcTextSize(text);
            var textPosition = screenPos with { Y = screenPos.Y - textSize.Y / 2 };

            Graphics.DrawTextWithBackground(text, textPosition, color, FontAlign.Center, Color.Black with { A = 200 });
            Graphics.DrawFilledCircleInWorld(worldPos, radius, color with { A = 150 }, segments);
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
