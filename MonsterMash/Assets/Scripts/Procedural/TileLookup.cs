using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileLookup", menuName = "ScriptableObjects/TileLookup")]
public class TileLookup : ScriptableObject
{

    [System.Serializable]
    public class EnumToTile
    {
        public Room.eTiles EnumTile;
        public Tile TileTile;
    }

    public List<EnumToTile> TileList = new List<EnumToTile>();
    public List<EnumToTile> OutdoorTileList = new List<EnumToTile>();
    public List<EnumToTile> IndoorTileList = new List<EnumToTile>();
    public RuleTile OutdoorBase;
    public RuleTile IndoorBase;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
