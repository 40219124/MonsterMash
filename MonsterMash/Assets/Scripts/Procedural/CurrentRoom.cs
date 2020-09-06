using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class CurrentRoom : MonoBehaviour
{
    public enum ETileLayer { none = -1, Base, Decoration, Foreground }
    public Tilemap BaseLayer;
    public Tilemap BaseDecoration;
    public Tilemap Foreground;

    public TileLookup TileTable;

    [SerializeField]
    private MapRoom ThisRoom;

	[Flags]
	public enum ETileContentType 
	{ Clear = 1, blocked=2, Door=4, Player=8, PlayerDestination=16, Enemy=32, EnemyDestination=64}

	public ETileContentType[,] TileContent;

    private void Start()
    {
		var mapRoom = ProceduralDungeon.Instance.GetCurrentRoom();
        SetRoom(mapRoom);
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
				int index = (9-y)*Settings.MapSize + x;
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
				TileContent[x,y] = type;
			}
		}
	}

    private void AssignTiles()
    {
        int index = 0;
        foreach (Room.eTiles tile in ThisRoom.RoomData.Tiles)
        {
            switch (tile)
            {
                /*case Room.eTiles.Wall:

                    break;
                case Room.eTiles.Floor:
                    break;
                case Room.eTiles.Door:
                    break;
                case Room.eTiles.Table:
                    break;
                case Room.eTiles.Hole:
                    break;
                case Room.eTiles.River:
                    break;
                case Room.eTiles.Enemy:
                    break;
                case Room.eTiles.Boss:
                    break;*/
                default:
                    BaseLayer.SetTile(new Vector3Int(index % 10, 8 - (index / 10), 0), TileTable.OutdoorBase);
                    break;
            }
            index++;
        }
        //BaseLayer.RefreshAllTiles();
        //BaseLayer.SetTile(Vector3Int.up * 8, null);
    }
}
