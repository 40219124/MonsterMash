using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Xml.Serialization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

public class CurrentRoom : MonoBehaviour
{
    public static CurrentRoom Instance;
    public enum EDoorPos { none = -1, TopLeft, TopRight, RightTop, RightBottom, BottomRight, BottomLeft, LeftBottom, LeftTop }
    public enum ETileLayer { none = -1, Base, Decoration, Foreground }
    public Tilemap BaseLayer;
    public Tilemap BaseDecoration;
    public Tilemap DecorationForeground;
    public Tilemap Foreground;

    public TileLookup TileTable;

    [SerializeField]
    private MapRoom ThisRoom;

    [Flags]
    public enum ETileContentType
    {
        Clear = 0, Blocked = 1, Door = 2, Player = 4, PlayerDestination = 8, Enemy = 16, EnemyDestination = 32,
        Impassable = Blocked | Player | PlayerDestination | Enemy | EnemyDestination
    }


    public ETileContentType[,] TileContent;

    private EnemySpawner EnemySpawn;
    private Vector3Int BossSpawnLocation = new Vector3Int(5, 4, 0);

    private List<Vector2Int> DoorLocs = new List<Vector2Int>();

    [SerializeField] CollectableObject HealPotion;
    [SerializeField] CollectableObject BossReward;


    void Awake()
    {
        HealPotion.gameObject.SetActive(false);
        BossReward.gameObject.SetActive(false);
        Instance = this;
    }

    public void Setup(bool loadedFromAnotherRoom)
    {
        EnemySpawn = GetComponent<EnemySpawner>();

        var mapRoom = ProceduralDungeon.Instance.GetCurrentRoom();
        if (ProceduralDungeon.Instance.IsLastRoom(mapRoom))
        {
            mapRoom.RoomData = ProceduralDungeon.Instance.AllRoomsData.GetBossRoom(mapRoom.RoomData.Area);
            ProceduralDungeon.Instance.MarkRoomAsBoss();
        }
        SetRoom(mapRoom);

        Player p = FindObjectOfType<Player>();
        p.Profile = OverworldMemory.GetCombatProfile(true);

        var playerPos = new Vector3(5, 4, 0);
        var targetPos = playerPos;
        if (loadedFromAnotherRoom)
        {
            playerPos = ProceduralDungeon.Instance.EnterPos;
            targetPos = ProceduralDungeon.Instance.EnterMoveTargetPos;
        }
        else if (!ThisRoom.IsStartingRoom)
        {
            playerPos = OverworldMemory.GetPlayerPosition();
            targetPos = playerPos;
        }
        p.transform.position = playerPos;
        p.MoveTarget = targetPos;

        if (ThisRoom.RoomState != ERoomState.Completed &&
        !ThisRoom.IsStartingRoom)
        {
            EnemySpawn.SpawnEnemies(ThisRoom.RoomState != ERoomState.NotSeen);
        }

        ThisRoom.RoomState = ERoomState.Seen;

        if (OverworldMemory.GetEnemyProfiles().Count == 0)
        {
            PlaceDoors();
        }
        PlaceCollectableItems();
    }
    public void SetRoom(MapRoom room)
    {
        ThisRoom = room;
        AssignTiles();
    }

    private void AssignTiles()
    {
        TileContent = new ETileContentType[Room.GameWidth, Room.GameHeight];
        EnemySpawn.SpawnLocations.Clear();
        int index = 0;
        foreach (Room.eTiles tile in ThisRoom.RoomData.Tiles)
        {
            Vector2Int pos = new Vector2Int(index % 10, 8 - index / 10);

            ETileContentType type = ETileContentType.Clear;
            switch (tile)
            {
                case Room.eTiles.Wall:
                    // Every tile places a wall :shrug:
                    type = ETileContentType.Blocked;
                    break;
                case Room.eTiles.Floor:
                    // As above :double_shrug:
                    break;
                case Room.eTiles.Door:
                    {
                        List<Tile> doorList = (ThisRoom.RoomData.Area == Room.eArea.Indoors ? TileTable.IndoorDoorsClosed : TileTable.OutdoorDoorsClosed);
                        if (ThisRoom.PositionIsDoor(index))
                        {
                            DoorLocs.Add(pos);
                            type = ETileContentType.Door;
                            BaseDecoration.SetTile((Vector3Int)pos, doorList[(int)EnumFromVector(pos)]);
                        }
                        else
                        {
                            type = ETileContentType.Blocked;
                        }
                        break;
                    }
                case Room.eTiles.Table:
                    {
                        BaseDecoration.SetTile((Vector3Int)pos, TileTable.Table);
                        type = ETileContentType.Blocked;
                        int upIndex = index - Room.GameWidth;
                        if (ThisRoom.RoomData.Tiles[upIndex] != Room.eTiles.Table)
                        {
                            Vector2Int upPos = pos + Vector2Int.up;

                            BaseDecoration.SetTile((Vector3Int)upPos, TileTable.Table);
                            Foreground.SetTile((Vector3Int)upPos, TileTable.TableTopSide);
                        }
                        break;
                    }
                case Room.eTiles.Hole:
                    BaseDecoration.SetTile((Vector3Int)pos, TileTable.Hole);
                    type = ETileContentType.Blocked;
                    break;
                case Room.eTiles.River:
                    if (ThisRoom.RoomData.Tiles[index - Room.GameWidth] != Room.eTiles.River)
                    {
                        BaseDecoration.SetTile((Vector3Int)pos, TileTable.RiverTop);
                    }
                    else if (ThisRoom.RoomData.Tiles[index + Room.GameWidth] != Room.eTiles.River)
                    {
                        BaseDecoration.SetTile((Vector3Int)pos, TileTable.RiverBottom);
                    }
                    else
                    {
                        BaseDecoration.SetTile((Vector3Int)pos, TileTable.River);
                    }
                    type = ETileContentType.Blocked;
                    break;
                case Room.eTiles.Enemy:
                    if (ThisRoom.IsBossRoom)
                    {
                        break;
                    }
                    // ~~~ add random chance for variation
                    EnemySpawn.SpawnLocations.Add(new EnemySpawner.MonsterPosition() { type = GetRandomEnemy(), pos = (Vector3Int)pos });
                    break;
                case Room.eTiles.Boss:
                    break;
                default:
                    break;
            }
            TileContent[pos.x, pos.y] = type;
            BaseLayer.SetTile((Vector3Int)pos, (ThisRoom.RoomData.Area == Room.eArea.Outdoors ? TileTable.OutdoorBase : TileTable.IndoorBase));

            PlaceRandomDecoration(type, pos, tile);
            TryAddCollectableItems(type, pos, tile);

            index++;
        }
        if (ThisRoom.RoomData.Area == Room.eArea.Outdoors)
        {
            PlaceTreeFluff();
        }
        if (ThisRoom.IsBossRoom)
        {
            switch (ThisRoom.RoomData.Area)
            {
                case Room.eArea.Outdoors:
                    EnemySpawn.SpawnLocations.Add(new EnemySpawner.MonsterPosition() { type = EMonsterType.lobster, pos = BossSpawnLocation });
                    break;
                case Room.eArea.Indoors:
                    EnemySpawn.SpawnLocations.Add(new EnemySpawner.MonsterPosition() { type = EMonsterType.shrimp, pos = BossSpawnLocation });
                    break;
                default:
                    break;
            }
            /*if (ThisRoom.RoomState == ERoomState.Completed)
            {
                TileContent[BossSpawnLocation.x - 1, BossSpawnLocation.y + 1] = ETileContentType.Clear;
                TileContent[BossSpawnLocation.x - 1, BossSpawnLocation.y] = ETileContentType.Clear;
                TileContent[BossSpawnLocation.x, BossSpawnLocation.y + 1] = ETileContentType.Clear;
                TileContent[BossSpawnLocation.x, BossSpawnLocation.y - 1] = ETileContentType.Clear;
            }
            else
            {
                TileContent[BossSpawnLocation.x - 1, BossSpawnLocation.y + 1] = ETileContentType.Enemy;
                TileContent[BossSpawnLocation.x - 1, BossSpawnLocation.y] = ETileContentType.Enemy;
                TileContent[BossSpawnLocation.x, BossSpawnLocation.y + 1] = ETileContentType.Enemy;
                TileContent[BossSpawnLocation.x, BossSpawnLocation.y - 1] = ETileContentType.Enemy;
            }*/
        }
    }

    void TryAddCollectableItems(ETileContentType type, Vector2Int pos, Room.eTiles tileType)
    {
        if (ThisRoom.CollectableItems.Count > 0 ||
            tileType != Room.eTiles.Floor ||
            ThisRoom.IsStartingRoom)
        {
            return;
        }

        if (UnityEngine.Random.Range(0f, 100f) <= Settings.ChanceOfHealingPotion)
        {
            var healingPotion = new HealingPotion(pos);

            ThisRoom.CollectableItems.Add(healingPotion);
        }
    }

    private void PlaceRandomDecoration(ETileContentType type, Vector2Int pos, Room.eTiles tileType)
    {

        float potionChance = 25.0f * (tileType == Room.eTiles.Table ? 1 : 0);
        float flowerChance = 4.0f * ((tileType == Room.eTiles.Floor && ThisRoom.RoomData.Area == Room.eArea.Outdoors) ? 1 : 0) + potionChance;
        float grassChance = 6.0f * ((tileType == Room.eTiles.Floor && ThisRoom.RoomData.Area == Room.eArea.Outdoors) ? 1 : 0) + flowerChance;

        if (grassChance == 0.0f)
        {
            return;
        }

        float rand = UnityEngine.Random.Range(0, 100.0f);

        ERoomDecoration decoration = ERoomDecoration.None;
        if (rand <= potionChance)
        {
            switch (UnityEngine.Random.Range(0, 2))
            {
                case 0:
                    decoration = ERoomDecoration.Potion1;
                    break;
                case 1:
                    decoration = ERoomDecoration.Potions2;
                    break;
                default:
                    break;
            }
        }
        else if (rand <= flowerChance)
        {
            decoration = ERoomDecoration.Flowers;
        }
        else if (rand <= grassChance)
        {
            switch (UnityEngine.Random.Range(0, 3))
            {
                case 0:
                    decoration = ERoomDecoration.BrownGrass1;
                    break;
                case 1:
                    decoration = ERoomDecoration.BrownGrass2;
                    break;
                case 2:
                    decoration = ERoomDecoration.BrownGrass3;
                    break;
                default:
                    break;
            }
        }

        Tile tile = TileTable.Decorations.Find(d => d.Decoration == decoration)?.DecTile;
        if (tile != null)
        {
            DecorationForeground.SetTile((Vector3Int)pos, tile);
        }
    }

    private EMonsterType GetRandomEnemy()
    {
        float slugChance = 0.0f;
        float mantisChance = 0.0f;
        float skeletonChance = 0.0f;
        switch (ThisRoom.RoomData.Area)
        {
            case Room.eArea.Outdoors:
                {
                    slugChance = 40.0f;
                    mantisChance = 40.0f + slugChance;
                    skeletonChance = 20.0f + mantisChance;
                    break;
                }
            case Room.eArea.Indoors:
                {
                    slugChance = 20.0f;
                    mantisChance = 30.0f + slugChance;
                    skeletonChance = 50.0f + mantisChance;
                    break;
                }
            default:
                break;
        }

        float rand = UnityEngine.Random.Range(0, skeletonChance);
        if (rand <= slugChance)
        {
            return EMonsterType.slug;
        }
        else if (rand <= mantisChance)
        {
            return EMonsterType.mantis;
        }
        else if (rand <= skeletonChance)
        {
            return EMonsterType.skeleton;
        }

        return EMonsterType.Frankenstein;
    }

    public void SetAsMoveTarget(Vector3 target, bool IsPlayer = false)
    {
        ETileContentType state = (IsPlayer ? ETileContentType.PlayerDestination : ETileContentType.EnemyDestination);
        TileContent[(int)target.x, (int)target.y] |= state;
    }
    public void SetAsActorPos(Vector3 target, bool IsPlayer = false)
    {
        ETileContentType dest = (IsPlayer ? ETileContentType.PlayerDestination : ETileContentType.EnemyDestination);
        ETileContentType pos = (IsPlayer ? ETileContentType.Player : ETileContentType.Enemy);
        TileContent[(int)target.x, (int)target.y] &= ~dest;
        TileContent[(int)target.x, (int)target.y] |= pos;
    }
    public void MoveActorFrom(Vector3 pos, bool IsPlayer = false)
    {
        ETileContentType dest = (IsPlayer ? ETileContentType.PlayerDestination : ETileContentType.EnemyDestination);
        ETileContentType posE = (IsPlayer ? ETileContentType.Player : ETileContentType.Enemy);
        TileContent[(int)pos.x, (int)pos.y] &= ~posE;
        TileContent[(int)pos.x, (int)pos.y] &= ~dest;
    }
    public void PlaceDoors()
    {
        List<Tile> doorList = (ThisRoom.RoomData.Area == Room.eArea.Indoors ? TileTable.IndoorDoorsOpen : TileTable.OutdoorDoorsOpen);
        ThisRoom.RoomState = ERoomState.Completed;
        foreach (Vector2Int pos in DoorLocs)
        {
            BaseDecoration.SetTile((Vector3Int)pos, doorList[(int)EnumFromVector(pos)]);
        }
    }

    public void PlaceTreeFluff()
    {
        int xMin = 1;
        int xMax = Room.GameWidth - 2;
        int yMin = 1;
        int yMax = Room.GameHeight - 2;
        for (int y = yMin; y <= yMax; ++y)
        {
            for (int x = xMin; x <= xMax; ++x)
            {
                EEightDirections dir = GetEightDirections(new Vector2Int(x, y), xMin, xMax, yMin, yMax);
                if (!IsNextToDoor(new Vector2Int(x, y)) && dir != EEightDirections.none)
                {
                    DecorationForeground.SetTile(new Vector3Int(x, y, 0), TileTable.TreeFluff.Find(t => t.Direction == dir).TileTile);
                }
            }
        }
    }

    public bool IsNextToDoor(Vector2Int pos)
    {

        Room.eDoorPlaces places = ThisRoom.UsedDoorPlaces;

        if (places.HasFlag(Room.eDoorPlaces.Top) && (pos == new Vector2Int(4, 7) || pos == new Vector2Int(5, 7)))
        {
            return true;
        }
        if (places.HasFlag(Room.eDoorPlaces.Right) && (pos == new Vector2Int(8, 4) || pos == new Vector2Int(8, 5)))
        {
            return true;
        }
        if (places.HasFlag(Room.eDoorPlaces.Bottom) && (pos == new Vector2Int(4, 1) || pos == new Vector2Int(5, 1)))
        {
            return true;
        }
        if (places.HasFlag(Room.eDoorPlaces.Left) && (pos == new Vector2Int(1, 4) || pos == new Vector2Int(1, 5)))
        {
            return true;
        }


        return false;
    }

    public EEightDirections GetEightDirections(Vector2Int pos, int xMin, int xMax, int yMin, int yMax)
    {
        if (!((pos.x == xMin || pos.x == xMax) || (pos.y == yMin || pos.y == yMax)))
        {
            return EEightDirections.none;
        }
        if (pos.x == xMin && pos.y == yMin)
        {
            return EEightDirections.SW;
        }
        else if (pos.x == xMin && pos.y == yMax)
        {
            return EEightDirections.NW;
        }
        else if (pos.x == xMax && pos.y == yMin)
        {
            return EEightDirections.SE;
        }
        else if (pos.x == xMax && pos.y == yMax)
        {
            return EEightDirections.NE;
        }
        else if (pos.x == xMin)
        {
            return EEightDirections.W;
        }
        else if (pos.x == xMax)
        {
            return EEightDirections.E;
        }
        else if (pos.y == yMin)
        {
            return EEightDirections.S;
        }
        else if (pos.y == yMax)
        {
            return EEightDirections.N;
        }
        return EEightDirections.none;
    }
    public void PlaceCollectableItems()
    {
        HealPotion.gameObject.SetActive(false);
        BossReward.gameObject.SetActive(false);

        if (ThisRoom.CollectableItems.Count > 2)
        {
            MMLogger.LogError("Too many CollectableItems");
        }

        foreach (var item in ThisRoom.CollectableItems)
        {
            if (item is HealingPotion)
            {
                MMLogger.Log($"added HealingPotion: {item}");
                HealPotion.Setup(item, ThisRoom.RoomState != ERoomState.Completed);
            }
            else if (item is BossPrize)
            {
                MMLogger.Log($"added HealingPotion: {item}");
                BossReward.Setup(item, ThisRoom.RoomState != ERoomState.Completed);
            }
        }
    }

    public void TryCollectItems(Vector3 playerPos)
    {
        HealPotion.PickUp(playerPos);
        BossReward.PickUp(playerPos);
    }

    private EDoorPos EnumFromVector(Vector2Int pos)
    {
        EDoorPos outEnum = EDoorPos.none;
        int index = pos.x + (Room.GameHeight - 1 - pos.y) * Room.GameWidth;

        if (index == Room.DoorPlaceTop1)
        {
            outEnum = EDoorPos.TopLeft;
        }
        else if (index == Room.DoorPlaceTop2)
        {
            outEnum = EDoorPos.TopRight;
        }
        else if (index == Room.DoorPlaceRight2)
        {
            outEnum = EDoorPos.RightTop;
        }
        else if (index == Room.DoorPlaceRight1)
        {
            outEnum = EDoorPos.RightBottom;
        }
        else if (index == Room.DoorPlaceBottom2)
        {
            outEnum = EDoorPos.BottomRight;
        }
        else if (index == Room.DoorPlaceBottom1)
        {
            outEnum = EDoorPos.BottomLeft;
        }
        else if (index == Room.DoorPlaceLeft1)
        {
            outEnum = EDoorPos.LeftBottom;
        }
        else if (index == Room.DoorPlaceLeft2)
        {
            outEnum = EDoorPos.LeftTop;
        }

        return outEnum;
    }
}
