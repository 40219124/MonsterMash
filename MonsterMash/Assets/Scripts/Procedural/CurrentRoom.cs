using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class CurrentRoom : MonoBehaviour
{
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
    { Clear = 1, blocked = 2, Door = 4, Player = 8, PlayerDestination = 16, Enemy = 32, EnemyDestination = 64 }

    public ETileContentType[,] TileContent;

    private EnemySpawner EnemySpawn;

    private List<Vector2Int> DoorLocs = new List<Vector2Int>();

    private void Start()
    {
        var mapRoom = ProceduralDungeon.Instance.GetCurrentRoom();
        SetRoom(mapRoom);
        EnemySpawn = GetComponent<EnemySpawner>();
        AssignTiles(); // ~~~ debug only
    }
    public void SetRoom(MapRoom room)
    {
        ThisRoom = room;
        AssignTiles();
        BuildTileContent();
    }

    void BuildTileContent()
    {
        var tiles = ThisRoom.RoomData.Tiles;
        TileContent = new ETileContentType[Settings.MapSize, Settings.MapSize];

        for (int x = 0; x < Settings.MapSize; x++)
        {
            for (int y = 0; y < Settings.MapSize; y++)
            {
                int index = (9 - y) * Settings.MapSize + x;
                ETileContentType type = ETileContentType.Clear;
                switch (tiles[index])
                {
                    case Room.eTiles.Wall:
                        type = ETileContentType.blocked;
                        break;
                    case Room.eTiles.Hole:
                        type = ETileContentType.blocked;
                        break;
                    case Room.eTiles.River:
                        type = ETileContentType.blocked;
                        break;
                    case Room.eTiles.Table:
                        type = ETileContentType.blocked;
                        break;
                    default:
                        type = ETileContentType.Clear;
                        break;
                }
                TileContent[x, y] = type;
            }
        }
    }

    private void AssignTiles()
    {
        EnemySpawn.SpawnLocations.Clear();
        int index = 0;
        foreach (Room.eTiles tile in ThisRoom.RoomData.Tiles)
        {
            switch (tile)
            {
                case Room.eTiles.Wall:
                    // Every tile places a wall :shrug:
                    break;
                case Room.eTiles.Floor:
                    // As above :double_shrug:
                    break;
                case Room.eTiles.Door:
                    // ~~~ help
                    Vector2Int pos = new Vector2Int(index % 10, index / 10);
                    if (ThisRoom.PositionIsDoor(pos))
                    {
                        DoorLocs.Add(pos);
                    }
                    break;
                case Room.eTiles.Table:

                    break;
                case Room.eTiles.Hole:
                    break;
                case Room.eTiles.River:
                    break;
                case Room.eTiles.Enemy:
                    // ~~~ add random chance for variation
                    EnemySpawn.SpawnLocations.Add(new EnemySpawner.MonsterPosition() { type = ThisRoom.RoomData.Area == Room.eArea.Outdoors ? EMonsterType.mantis : EMonsterType.skeleton });
                    break;
                case Room.eTiles.Boss:
                    break;
                default:
                    break;
            }
            BaseLayer.SetTile(new Vector3Int(index % 10, 8 - (index / 10), 0), TileTable.OutdoorBase);
            index++;
        }
        //BaseLayer.RefreshAllTiles();
        //BaseLayer.SetTile(Vector3Int.up * 8, null);
    }

    public void PlaceDoors()
    {
        foreach (Vector2Int pos in DoorLocs)
        {
            BaseDecoration.SetTile((Vector3Int)pos, TileTable.OutdoorDoors[(int)EDoorPos.TopLeft]);
        }
    }
}
