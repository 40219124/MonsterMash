using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RoomEditor : EditorWindow
{
    enum eWalls
    {
        Top = 0,
        TR = 1,
        Right = 2,
        BR = 3,
        Bottom = 4,
        BL = 5,
        Left = 6,
        TL = 7,
    }


    static AllRoomData AllRooms;
    static string jsonFile = "/RoomData.json";
    Vector2 ScrollPos;
    int EditingRoom;
    bool EditingMode;

    static Texture2D Floor;
    static Texture2D[] Walls = new Texture2D[8];
    Texture2D Enemy;

    Texture2D FloorIndoor;
    Texture2D[] WallsIndoor = new Texture2D[8];
    Texture2D EnemyIndoor;

    Texture2D Table;
    Texture2D Hole;
    Texture2D River;
     
    static Room.eTiles PlacingTile = Room.eTiles.Floor;

    [MenuItem("MonsterMash/Room Editor")]
    static void Init()
    {
        RoomEditor window = (RoomEditor)EditorWindow.GetWindow(typeof(RoomEditor));


        window.LoadRooms();

        window.Show();
    }

    void LoadResources()
    {
        Floor = Resources.Load("OutdoorBase") as Texture2D;
        Walls[0] = Resources.Load("Outdoor - top") as Texture2D;
        Walls[1] = Resources.Load("Outdoor - Corner - TR") as Texture2D;
        Walls[2] = Resources.Load("Outdoor - Right") as Texture2D;
        Walls[3] = Resources.Load("Outdoor - Corner - BR") as Texture2D;
        Walls[4] = Resources.Load("Outdoor - Bottom") as Texture2D;
        Walls[5] = Resources.Load("Outdoor - Corner - BL") as Texture2D;
        Walls[6] = Resources.Load("Outdoor - Left") as Texture2D;
        Walls[7] = Resources.Load("Outdoor - Corner - TL") as Texture2D;
        Enemy = Resources.Load("mantis_btn") as Texture2D;

        FloorIndoor = Resources.Load("Floor_1") as Texture2D;
        WallsIndoor[0] = Resources.Load("Wall - top") as Texture2D;
        WallsIndoor[1] = Resources.Load("Wall - Corner - tr") as Texture2D;
        WallsIndoor[2] = Resources.Load("Wall - right") as Texture2D;
        WallsIndoor[3] = Resources.Load("Wall - Corner - br") as Texture2D;
        WallsIndoor[4] = Resources.Load("Wall - Bottom") as Texture2D;
        WallsIndoor[5] = Resources.Load("Wall - Corner - bl") as Texture2D;
        WallsIndoor[6] = Resources.Load("Wall - leftt") as Texture2D;
        WallsIndoor[7] = Resources.Load("Wall - Corner - tl") as Texture2D;
        EnemyIndoor = Resources.Load("skelly_btn") as Texture2D;


        Table = Resources.Load("TableMiddle") as Texture2D;
        Hole = Resources.Load("Hole") as Texture2D;
        River = Resources.Load("River") as Texture2D;
    }

    void OnGUI()
    {
        if (EditingMode)
        {
            ShowEditing();
        }
        else
        {
            ShowNormal();
        }
    }

    void ShowNormal()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Save"))
        {
            SaveRooms();
        }

        if (GUILayout.Button("New Room"))
        {
            NewRoom();
        }

        if (GUILayout.Button("Reload"))
        {
            LoadRooms();
        }

        GUILayout.EndHorizontal();

        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);

        if (AllRooms != null)
        {
            for (int i = 0; i < AllRooms.AllRooms.Count; ++i)
            {
                Room room = AllRooms.AllRooms[i];
                ShowRoomData(ref room);
                AllRooms.AllRooms[i] = room;
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Edit Room"))
                {
                    LoadEditingMode(i);
                }

                if (GUILayout.Button("Delete Room"))
                {
                    AllRooms.AllRooms.Remove(room);
                }

                GUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
    }


    void ShowEditing()
    {
        if (AllRooms == null)
        {
            EditingMode = false;
            return;
        }
        Room room = AllRooms.AllRooms[EditingRoom];
        GUILayout.Label($"Currently Editing Room: {room.RoomId}");
        if (GUILayout.Button("Finish Editing"))
        {
            EditingMode = false;
            AllRooms.AllRooms[EditingRoom] = room;
        }
        ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);

        GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
        {
            fixedHeight = 0,
            fixedWidth = 0,
            stretchWidth = true,
            stretchHeight = true,
            margin = new RectOffset(2, 2, 2, 2),
        };

        ///Button selection
        GUILayout.BeginHorizontal();

        Texture2D texture = null;
        btnStyle.normal.background = room.Area == Room.eArea.Outdoors ? Floor : FloorIndoor;
        texture = PlacingTile == Room.eTiles.Floor ? btnStyle.normal.background : null;
        if (GUILayout.Button(texture, btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
        {
            PlacingTile = Room.eTiles.Floor;
        }

        btnStyle.normal.background = room.Area == Room.eArea.Outdoors ? Walls[(int)eWalls.TL] : WallsIndoor[(int)eWalls.TL];
        texture = PlacingTile == Room.eTiles.Wall ? btnStyle.normal.background : null;
        if (GUILayout.Button(texture, btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
        {
            PlacingTile = Room.eTiles.Wall;
        }

        btnStyle.normal.background = room.Area == Room.eArea.Outdoors ? Enemy : EnemyIndoor;
        texture = PlacingTile == Room.eTiles.Enemy ? btnStyle.normal.background : null;
        if (GUILayout.Button(texture, btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
        {
            PlacingTile = Room.eTiles.Enemy;
        }

        btnStyle.normal.background = Table;
        texture = PlacingTile == Room.eTiles.Table ? btnStyle.normal.background : null;
        if (GUILayout.Button(texture, btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
        {
            PlacingTile = Room.eTiles.Table;
        }

        btnStyle.normal.background = Hole;
        texture = PlacingTile == Room.eTiles.Hole ? btnStyle.normal.background : null;
        if (GUILayout.Button(texture, btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
        {
            PlacingTile = Room.eTiles.Hole;
        }


        btnStyle.normal.background = River;
        texture = PlacingTile == Room.eTiles.River ? btnStyle.normal.background : null;
        if (GUILayout.Button(texture, btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
        {
            PlacingTile = Room.eTiles.River;
        }
        GUILayout.EndHorizontal();
        ///End button selection

        ///Level Drawing
        for (int i = 0; i < Room.GameHeight; ++i)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space((position.width - 500) / 2.0f);
            for (int j = 0; j < Room.GameWidth; ++j)
            {
                int tileIndex = i * Room.GameWidth + j;
                GUI.enabled = room.Tiles[tileIndex] != Room.eTiles.Door;
                btnStyle.normal.background = GetTileSprite(room.Tiles[tileIndex], new Vector2(j, i), room.Area);
                if (GUILayout.Button($"", btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
                {
                    MMLogger.Log($"Editing: {j}, {i}");
                    room.Tiles[tileIndex] = PlacingTile;
                }
            }
            GUILayout.EndHorizontal();
        }
        GUI.enabled = true;
        EditorGUILayout.EndScrollView();
        //End Level Drawing
    }

    Texture2D GetTileSprite(Room.eTiles tile, Vector2 tileId, Room.eArea area)
    {
        switch (tile)
        {
            case Room.eTiles.Floor:
                return area == Room.eArea.Outdoors ? Floor : FloorIndoor;
            case Room.eTiles.Door:
                return area == Room.eArea.Outdoors ? Floor : FloorIndoor;
            case Room.eTiles.Enemy:
                return area == Room.eArea.Outdoors ? Enemy : EnemyIndoor;
            case Room.eTiles.Table:
                return Table;
            case Room.eTiles.Hole:
                return Hole;
            case Room.eTiles.River:
                return River;
            case Room.eTiles.Wall:
                if (tileId == Vector2.zero)
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.TL] : WallsIndoor[(int)eWalls.TL];
                }
                else if (tileId == new Vector2(0, Room.GameHeight - 1))
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.BL] : WallsIndoor[(int)eWalls.BL];
                }
                else if (tileId == new Vector2(Room.GameWidth - 1, 0))
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.TR] : WallsIndoor[(int)eWalls.TR];
                }
                else if (tileId == new Vector2(Room.GameWidth - 1, Room.GameHeight - 1))
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.BR] : WallsIndoor[(int)eWalls.BR];
                }
                else if (tileId.y == 0)
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.Top] : WallsIndoor[(int)eWalls.Top];
                }
                else if (tileId.y == Room.GameHeight - 1)
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.Bottom] : WallsIndoor[(int)eWalls.Bottom];
                }
                else if (tileId.x == 0)
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.Left] : WallsIndoor[(int)eWalls.Left];
                }
                else if (tileId.x == Room.GameWidth - 1)
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.Right] : WallsIndoor[(int)eWalls.Right];
                }
                else
                {
                    return area == Room.eArea.Outdoors ? Walls[(int)eWalls.TL] : WallsIndoor[(int)eWalls.TL];
                }
        }

        return Floor;
    }

    void LoadEditingMode(int i)
    {
        EditingMode = true;
        EditingRoom = i;
    }

    void OnFocus()
    {
        if (!EditingMode)
        {
            LoadRooms();
        }
    }

    void LoadRooms()
    {
        string path = Application.streamingAssetsPath + jsonFile;

        if (File.Exists(path))
        {
            MMLogger.Log($"Loading from: {path}");

            var roomData = File.ReadAllText(path);

            AllRooms = JsonUtility.FromJson<AllRoomData>(roomData);

            if (AllRooms == null)
            {
                AllRooms = new AllRoomData();
            }
            else
            {
                AllRooms.Load();
            }
        }
        else
        {
            AllRooms = new AllRoomData();
        }

        LoadResources();
    }

    static void SaveRooms()
    {
        string path = Application.streamingAssetsPath + jsonFile;
        MMLogger.Log($"saving to: {path}");

        if (!File.Exists(path))
        {
            MMLogger.Log("creating path");
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        AllRooms.Save();
        File.WriteAllText(path, JsonUtility.ToJson(AllRooms));
        MMLogger.Log("saved");
    }

    static void NewRoom()
    {
        if (AllRooms.AllRooms == null)
        {
            AllRooms.AllRooms = new List<Room>();
        }
        AllRooms.AllRooms.Add(new Room());
    }

    void ShowRoomData(ref Room room)
    {
        using (new GUILayout.HorizontalScope())
        {
                using (new GUILayout.HorizontalScope())
                {

                    GUILayout.Label("Room Id", EditorStyles.boldLabel);
                    GUILayout.Label($"{room.RoomId}");
                }

            using (new GUILayout.VerticalScope())
            {

                room.IsBossRoom = EditorGUILayout.Toggle("Boss Room", room.IsBossRoom);
                room.Area = (Room.eArea)EditorGUILayout.EnumPopup("Environment", room.Area);

                GUILayout.BeginHorizontal();
                bool top = EditorGUILayout.Toggle("Top", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Top));
                bool botttom = EditorGUILayout.Toggle("Bottom", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Bottom));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                bool left = EditorGUILayout.Toggle("Left", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Left));
                bool right = EditorGUILayout.Toggle("Right", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Right));
                GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.BeginVertical();
				room.PlayerSpawn = EditorGUILayout.Vector2Field("Player Spawn", room.PlayerSpawn);
				if (!CheckSpawnValid(room.PlayerSpawn))
				{
					EditorGUILayout.HelpBox("Player Spawn not set", MessageType.Error);
				}
				GUILayout.EndVertical();

				GUILayout.BeginVertical();
				room.HealingSpawn = Vector2ToInt(EditorGUILayout.Vector2Field("Healing Spawn", room.HealingSpawn));
				if (!CheckSpawnValid(room.HealingSpawn))
				{
					EditorGUILayout.HelpBox("Healing Spawn not set", MessageType.Error);
				}
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();

                SetRoomDoors(ref room, top, botttom, left, right);
            }

            using (new GUILayout.VerticalScope())
            {

                GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
                {
                    fixedHeight = 0,
                    fixedWidth = 0,
                    stretchWidth = true,
                    stretchHeight = true,
                    margin = new RectOffset(0, 0, 0, 0),
                };


                for (int i = 0; i < Room.GameHeight; ++i)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space((position.width - 500) / 2.0f);
                        for (int j = 0; j < Room.GameWidth; ++j)
                        {
                            btnStyle.normal.background = GetTileSprite(room.Tiles[i * Room.GameWidth + j], new Vector2(j, i), room.Area);
                            GUILayout.Button($"", btnStyle, GUILayout.Height(8), GUILayout.Width(8));
                        }
                    }
                }
            }
        }
        GUILayout.Space(20);
    }
	
	bool CheckSpawnValid(Vector2 spawnPos)
	{
		return spawnPos.x > 0 && spawnPos.x < Room.GameWidth-1 &&
				spawnPos.y > 0 && spawnPos.y < Room.GameHeight-1;
	}

	Vector2Int Vector2ToInt(Vector2 vec)
	{
		return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
	}

    static void SetRoomDoors(ref Room room, bool top, bool bottom, bool left, bool right)
    {
        Room.eDoorPlaces doorPlaces = Room.eDoorPlaces.None;
        room.Tiles[Room.DoorPlaceTop1] = Room.eTiles.Wall;
        room.Tiles[Room.DoorPlaceTop2] = Room.eTiles.Wall;
        room.Tiles[Room.DoorPlaceBottom1] = Room.eTiles.Wall;
        room.Tiles[Room.DoorPlaceBottom2] = Room.eTiles.Wall;
        room.Tiles[Room.DoorPlaceLeft1] = Room.eTiles.Wall;
        room.Tiles[Room.DoorPlaceLeft2] = Room.eTiles.Wall;
        room.Tiles[Room.DoorPlaceRight1] = Room.eTiles.Wall;
        room.Tiles[Room.DoorPlaceRight2] = Room.eTiles.Wall;

        if (top)
        {
            doorPlaces |= Room.eDoorPlaces.Top;
            room.Tiles[Room.DoorPlaceTop1] = Room.eTiles.Door;
            room.Tiles[Room.DoorPlaceTop2] = Room.eTiles.Door;
        }
        if (bottom)
        {
            doorPlaces |= Room.eDoorPlaces.Bottom;
            room.Tiles[Room.DoorPlaceBottom1] = Room.eTiles.Door;
            room.Tiles[Room.DoorPlaceBottom2] = Room.eTiles.Door;
        }
        if (left)
        {
            doorPlaces |= Room.eDoorPlaces.Left;
            room.Tiles[Room.DoorPlaceLeft1] = Room.eTiles.Door;
            room.Tiles[Room.DoorPlaceLeft2] = Room.eTiles.Door;
        }
        if (right)
        {
            doorPlaces |= Room.eDoorPlaces.Right;
            room.Tiles[Room.DoorPlaceRight1] = Room.eTiles.Door;
            room.Tiles[Room.DoorPlaceRight2] = Room.eTiles.Door;
        }

        room.DoorPlaces = doorPlaces;
    }
}
