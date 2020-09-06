using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class CurrentRoom : MonoBehaviour
{
    public static CurrentRoom Instance;
    public enum EDoorPos { none = -1, TopLeft, TopRight, RightTop, RightBottom, BottomRight, BottomLeft, LeftBottom, LeftTop }
    public enum ETileLayer { none = -1, Base, Decoration, Foreground }
    public Tilemap BaseLayer;
    public Tilemap BaseDecoration;
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

    private List<Vector2Int> DoorLocs = new List<Vector2Int>();

    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        EnemySpawn = GetComponent<EnemySpawner>();

        var mapRoom = ProceduralDungeon.Instance.GetCurrentRoom();
        SetRoom(mapRoom);

		if (ThisRoom.RoomState != ERoomState.NotSeen)
		{
			Player p = FindObjectOfType<Player>();
			p.transform.position = OverworldMemory.GetPlayerPosition();
			p.Profile = OverworldMemory.GetCombatProfile(true);
		}

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
                    // ~~~ help
                    if (ThisRoom.PositionIsDoor(index))
                    {
                        DoorLocs.Add(pos);
						type = ETileContentType.Door;
                    }
					else
					{
						type = ETileContentType.Blocked;
					}
                    break;
                case Room.eTiles.Table:
					type = ETileContentType.Blocked;
                    break;
                case Room.eTiles.Hole:
					type = ETileContentType.Blocked;
                    break;
                case Room.eTiles.River:
					type = ETileContentType.Blocked;
                    break;
                case Room.eTiles.Enemy:
                    // ~~~ add random chance for variation
                    EnemySpawn.SpawnLocations.Add(new EnemySpawner.MonsterPosition() { type = ThisRoom.RoomData.Area == Room.eArea.Outdoors ? EMonsterType.mantis : EMonsterType.skeleton, pos = (Vector3Int)pos });
                    break;
                case Room.eTiles.Boss:
                    break;
                default:
                    break;
            }
			TileContent[pos.x, pos.y] = type;
            BaseLayer.SetTile((Vector3Int)pos, TileTable.OutdoorBase);
            index++;
        }
        //BaseLayer.RefreshAllTiles();
        //BaseLayer.SetTile(Vector3Int.up * 8, null);
    }

    public void PlaceDoors()
    {
		ThisRoom.RoomState = ERoomState.Completed;
        foreach (Vector2Int pos in DoorLocs)
        {
            BaseDecoration.SetTile((Vector3Int)pos, TileTable.OutdoorDoors[(int)EnumFromVector(pos)]);
        }
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
