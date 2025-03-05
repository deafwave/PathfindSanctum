using System.Linq;
using System.Numerics;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Enums;
using ImGuiNET;

namespace PathfindSanctum;

public class EffectHelper(
    GameController gameController,
    Graphics graphics
)
{

    private void DrawHazard(string text, Vector2 screenPos, Vector3 worldPos, float radius, int segments, SharpDX.Color color = default)
    {
        if (color == default)
        {
            color = SharpDX.Color.Red;
        }

        var textSize = ImGui.CalcTextSize(text);
        var textPosition = screenPos with { Y = screenPos.Y - textSize.Y / 2 };

        graphics.DrawTextWithBackground(text, textPosition, color, FontAlign.Center, SharpDX.Color.Black with { A = 200 });
        graphics.DrawFilledCircleInWorld(worldPos, radius, color with { A = 150 }, segments);
    }

    private void DrawTerrainEffects()
    {
        var terrainEntityList = gameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Terrain] ?? [];

        foreach (var entity in terrainEntityList)
        {
            if (entity.DistancePlayer >= 100)
                continue;
                
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
                        DrawHazard("Spawner", pos, entity.PosNum, 60.0f, 4, SharpDX.Color.Lime);
                        break;
                    case false:
                        DrawHazard(" + ", pos, entity.PosNum, 20.0f, 4, SharpDX.Color.LightBlue);
                        break;
                }
            }
        }
    }

    private void DrawSkillEffects()
    {
        var effectEntityList = gameController?.EntityListWrapper?.ValidEntitiesByType[EntityType.Effect]
            .Where(x => x.Metadata.Contains("/Effects/Effect") &&
                        x.TryGetComponent<Animated>(out var animComp) &&
                        animComp?.BaseAnimatedObjectEntity.Metadata != null) ?? [];


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
    }

    public void DrawEffects()
    {
        DrawTerrainEffects();
        DrawSkillEffects();
    }
}
