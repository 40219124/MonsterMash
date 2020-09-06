using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CurrentRoom : MonoBehaviour
{
    public enum ETileLayer { none = -1, Base, Decoration, Foreground }
    public Tilemap BaseLayer;
    public Tilemap BaseDecoration;
    public Tilemap Foreground;

    public TileLookup TileTable;

    [SerializeField]
    private MapRoom ThisRoom;

    private void Start()
    {
        AssignTiles();
    }
    public void SetRoom(MapRoom room)
    {
        ThisRoom = room;
        AssignTiles();
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
