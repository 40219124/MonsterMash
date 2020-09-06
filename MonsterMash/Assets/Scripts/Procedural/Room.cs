using System;
using System.Collections.Generic;

[Serializable]
public class Room
{
    [Flags]
    public enum eDoorPlaces
    {
        None = 0,
        Top = 1 << 0,
        Bottom = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3
    }

    public enum eArea
    {
        Outdoors,
        Indoors
    }

    public enum eTiles
    {
        Wall,
        Floor,
        Door,
        Table,
        Hole,
        River,
        Enemy,
        Boss
    }

    public int RoomId;
    public eDoorPlaces DoorPlaces = eDoorPlaces.Left;
    public bool IsBossRoom;
    public eArea Area = eArea.Outdoors;
    public eTiles[] Tiles = new eTiles[90];


    public Room()
    {
        AllRoomData.StaticRoomID++;
        RoomId = AllRoomData.StaticRoomID;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Room room))
        {
            return false;
        }

        return RoomId == room.RoomId;
    }


    public bool DoorsMatch(eDoorPlaces required)
    {
        return (required & DoorPlaces) == required;
    }
}

[Serializable]
public class AllRoomData
{
    public static int StaticRoomID = 0;
    public int LatestRoomID = 0;
    public List<Room> AllRooms = new List<Room>();

    public void Load()
    {
        StaticRoomID = LatestRoomID;
    }

    public void Save()
    {
        LatestRoomID = StaticRoomID;
    }

    public List<Room> ValidRooms(Room.eDoorPlaces required, Room.eArea area)
    {
        List<Room> outRooms = new List<Room>();

        for(int i = 0; i < AllRooms.Count; ++i)
        {
            if (AllRooms[i].Area == area && AllRooms[i].DoorsMatch(required))
            {
                outRooms.Add(AllRooms[i]);
            }
        }

        return outRooms;
    }
}
