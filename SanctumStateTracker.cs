using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExileCore;
using ExileCore.PoEMemory.Elements.Sanctum;
using ExileCore.PoEMemory.FilesInMemory.Sanctum;

namespace PathfindSanctum;

public class SanctumStateTracker
{
    private uint? currentAreaHash;
    private Dictionary<(int Layer, int Room), RoomState> roomStates = new();

    public List<List<SanctumRoomElement>> roomsByLayer;
    public byte[][][] roomLayout;

    public int PlayerLayerIndex = -1;
    public int PlayerRoomIndex = -1;
    public int PlayerFloor = 1;
    public int PlayerResolve = 0;
    public int PlayerInspiration = 0;
    public int PlayerGold = 0;
    public int PlayerMaxResolve = 0;

    public bool HasRoomData()
    {
        return roomStates.Count > 0;
    }

    public bool IsSameSanctum(AreaInstance newArea)
    {
        var newAreaHash = newArea.Hash;
        var newAreaRawName = newArea.Area.RawName;
        if(newAreaRawName == "SanctumFoyer_1_1" || newAreaRawName == "SanctumFoyer_2_3" || newAreaRawName == "SanctumFoyer_3_3" || newAreaRawName == "SanctumFoyer_4_3")
        {
            // TODO: Dump data to file for analysis later
            // if all seeing eye on floor 3 foyer, dump room data to file?
            // if all seeing eye on floor 4 foyer, dump room data to file ?
            if (currentAreaHash == null)
            {
                currentAreaHash = newAreaHash;
                return false;
            }
            return currentAreaHash == newAreaHash;
        }
        return true;
    }

    private static int GetFloorNumber(string bossId)
    {
        return bossId switch
        {
            "Cellar_Boss_1_1" => 1,
            "Vaults_Boss_1_1" => 2,
            "Nave_Boss_1_1" => 3,
            "Crypt_Boss_1_1" => 4,
            _ => -1,
        };
    }

    public void UpdateRoomStates(SanctumFloorWindow floorWindow)
    {
        this.roomsByLayer = floorWindow.RoomsByLayer;
        if (roomsByLayer == null || roomsByLayer.Count == 0)
        {
            return;
        }

        // Update Layout Data
        this.roomLayout = floorWindow.FloorData.RoomLayout;

        // Update Player Data
        // FIXME: Minor OBO error where if the player selects a room then restarts the sanctum
        // the layer is considered done
        PlayerLayerIndex = floorWindow.FloorData.RoomChoices.Count - 1;
        PlayerRoomIndex =
            floorWindow.FloorData.RoomChoices.Count > 0
                ? floorWindow.FloorData.RoomChoices.Last()
                : -1;
        PlayerFloor = GetFloorNumber(floorWindow?.Rooms?.Last()?.Data?.FightRoom?.Id);
        PlayerResolve = floorWindow.FloorData.CurrentResolve;
        PlayerInspiration = floorWindow.FloorData.Inspiration;
        PlayerGold = floorWindow.FloorData.Gold;
        PlayerMaxResolve = floorWindow.FloorData.MaxResolve;

        // Update Room Data
        for (var layer = 0; layer < roomsByLayer.Count; layer++)
        {
            for (var room = 0; room < roomsByLayer[layer].Count; room++)
            {
                var sanctumRoom = roomsByLayer[layer][room];
                if (sanctumRoom == null)
                {
                    continue;
                }

                var key = (layer, room);
                if (!roomStates.ContainsKey(key))
                {
                    int numConnections = roomLayout[layer][room].Length;
                    roomStates[key] = new RoomState(sanctumRoom, numConnections);
                }
                else
                {
                    roomStates[key].UpdateRoom(sanctumRoom);
                }
            }
        }
    }

    public void Reset(AreaInstance newArea)
    {
        currentAreaHash = newArea.Hash;
        roomStates.Clear();
    }

    public RoomState GetRoom(int layer, int room)
    {
        return roomStates.TryGetValue((layer, room), out var state) ? state : null;
    }
}

public class RoomState
{
    public string FightType { get; private set; }
    public string Affliction { get; private set; }
    public string RewardType { get; private set; }
    public List<SanctumDeferredRewardCategory> Rewards { get; private set; }
    public int Connections { get; private set; }

    public SharpDX.Vector2 Position { get; internal set; }

    public RoomState(SanctumRoomElement room, int numConnections)
    {
        Connections = numConnections;
        UpdateRoom(room);
    }

    public void UpdateRoom(SanctumRoomElement newRoom)
    {
        var newFightType = newRoom.Data.FightRoom?.RoomType.Id;
        var newAffliction = newRoom.Data?.RoomEffect?.ReadableName;
        var newRewardType = newRoom.Data.RewardRoom?.RoomType.Id;
        var newRewards = newRoom.Data.Rewards;

        // Only update each field if we're getting new information (not null/empty)
        if (!string.IsNullOrEmpty(newFightType))
            FightType = newFightType;
        if (!string.IsNullOrEmpty(newAffliction))
            Affliction = newAffliction;
        if (!string.IsNullOrEmpty(newRewardType))
            RewardType = newRewardType;
        if (newRewards != null)
            Rewards = newRewards;
        //Position = newRoom.PositionNum; // FIXME: Diagnose why this doesn't work
        Position = newRoom.GetClientRect().TopLeft;
    }

    public override string ToString()
    {
        return $"FightType: {FightType}, Affliction: {Affliction}, RewardType: {RewardType}, Connections: {Connections}";
    }
}
