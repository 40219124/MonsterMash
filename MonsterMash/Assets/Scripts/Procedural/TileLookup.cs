using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EEightDirections { none = -1, N, NE, E, SE, S, SW, W, NW }

[CreateAssetMenu(fileName = "TileLookup", menuName = "ScriptableObjects/TileLookup")]
public class TileLookup : ScriptableObject
{

    [System.Serializable]
    public class EnumToTile
    {
        public Room.eTiles EnumTile;
        public Tile TileTile;
    }
    [System.Serializable]
    public class EnumToDecoration
    {
        public ERoomDecoration Decoration;
        public Tile DecTile;
    }
    [System.Serializable]
    public class EightDToTile
    {
        public EEightDirections Direction;
        public Tile TileTile;
    }

    public List<EnumToTile> TileList = new List<EnumToTile>();
    public List<EnumToTile> OutdoorTileList = new List<EnumToTile>();
    public List<EnumToTile> IndoorTileList = new List<EnumToTile>();
    public RuleTile OutdoorBase;
    public RuleTile IndoorBase;

    public List<Tile> OutdoorDoorsOpen = new List<Tile>();
    public List<Tile> OutdoorDoorsClosed = new List<Tile>();
    public List<Tile> IndoorDoorsOpen = new List<Tile>();
    public List<Tile> IndoorDoorsClosed = new List<Tile>();

    public AnimatedTile RiverTop;
    public AnimatedTile River;
    public AnimatedTile RiverBottom;

    public RuleTile Table;
    public RuleTile TableTopSide;

    public List<EnumToDecoration> Decorations = new List<EnumToDecoration>();

    public Tile Hole;

    public List<EightDToTile> TreeFluff = new List<EightDToTile>();
}
