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

    Texture2D Floor;
    Texture2D[] Walls = new Texture2D[8];

    [MenuItem("MonsterMash/Room Editor")]
    static void Init()
    {
        RoomEditor window = (RoomEditor)EditorWindow.GetWindow(typeof(RoomEditor));

        LoadRooms();

        window.Show();
    }
    private void OnGUI()
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

    int GameHeight = 8;
    int GameWidth = 10;

    void ShowEditing()
    {
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
            margin = new RectOffset(0, 1, 0, 2),
        };


        for (int i = 0; i < GameHeight; ++i)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space((position.width - 500) / 2.0f);
            for (int j = 0; j < GameWidth; ++j)
            {
                btnStyle.normal.background = GetTileSprite(room.Tiles[i * GameWidth + j], new Vector2(j, i));
                if (GUILayout.Button($"", btnStyle, GUILayout.Height(50), GUILayout.Width(50)))
                {
                    Debug.Log($"Editing: {j}, {i}");
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    Texture2D GetTileSprite(Room.eTiles tile, Vector2 tileId)
    {
        switch (tile)
        {
            case Room.eTiles.Floor:
                return Floor;
            case Room.eTiles.Wall:
                if (tileId == Vector2.zero)
                {
                    return Walls[(int)eWalls.TL];
                }
                else if (tileId == new Vector2(0, GameHeight - 1))
                {
                    return Walls[(int)eWalls.BL];
                }
                else if (tileId == new Vector2(GameWidth - 1, 0))
                {
                    return Walls[(int)eWalls.TR];
                }
                else if (tileId == new Vector2(GameWidth - 1, GameHeight - 1))
                {
                    return Walls[(int)eWalls.BR];
                }
                else if (tileId.y == 0)
                {
                    return Walls[(int)eWalls.Top];
                }
                else if (tileId.y == GameHeight - 1)
                {
                    return Walls[(int)eWalls.Bottom];
                }
                else if (tileId.x == 0)
                {
                    return Walls[(int)eWalls.Left];
                }
                else if (tileId.x == GameWidth - 1)
                {
                    return Walls[(int)eWalls.Right];
                }
                break;
        }

        return Floor;
    }

    void LoadEditingMode(int i)
    {
        EditingMode = true;
        EditingRoom = i;

        Floor = Resources.Load("OutdoorBase") as Texture2D;
        Walls[0] = Resources.Load("Outdoor - top") as Texture2D;
        Walls[1] = Resources.Load("Outdoor - Corner - TR") as Texture2D;
        Walls[2] = Resources.Load("Outdoor - Right") as Texture2D;
        Walls[3] = Resources.Load("Outdoor - Corner - BR") as Texture2D;
        Walls[4] = Resources.Load("Outdoor - Bottom") as Texture2D;
        Walls[5] = Resources.Load("Outdoor - Corner - BL") as Texture2D;
        Walls[6] = Resources.Load("Outdoor - Left") as Texture2D;
        Walls[7] = Resources.Load("Outdoor - Corner - TL") as Texture2D;
    }

    private void OnFocus()
    {
        LoadRooms();
    }

    static void LoadRooms()
    {
        string path = Application.streamingAssetsPath + jsonFile;

        if (File.Exists(path))
        {
            Debug.Log($"Loading from: {path}");

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
    }

    static void SaveRooms()
    {
        string path = Application.streamingAssetsPath + jsonFile;
        Debug.Log($"saving to: {path}");

        if (!File.Exists(path))
        {
            Debug.Log("creating path");
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        AllRooms.Save();
        File.WriteAllText(path, JsonUtility.ToJson(AllRooms));
        Debug.Log("saved");
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

                GUILayout.BeginHorizontal();
                bool top = EditorGUILayout.Toggle("Top", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Top));
                bool botttom = EditorGUILayout.Toggle("Bottom", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Bottom));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                bool left = EditorGUILayout.Toggle("Left", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Left));
                bool right = EditorGUILayout.Toggle("Right", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Right));
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


                for (int i = 0; i < GameHeight; ++i)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space((position.width - 500) / 2.0f);
                        for (int j = 0; j < GameWidth; ++j)
                        {
                            btnStyle.normal.background = GetTileSprite(room.Tiles[i * GameWidth + j], new Vector2(j, i));
                            GUILayout.Button($"", btnStyle, GUILayout.Height(8), GUILayout.Width(8));
                        }
                    }
                }
            }
        }
        GUILayout.Space(20);
    }

    static void SetRoomDoors(ref Room room, bool top, bool bottom, bool left, bool right)
    {
        Room.eDoorPlaces doorPlaces = Room.eDoorPlaces.None;

        if (top)
        {
            doorPlaces |= Room.eDoorPlaces.Top;
        }
        if (bottom)
        {
            doorPlaces |= Room.eDoorPlaces.Bottom;
        }
        if (left)
        {
            doorPlaces |= Room.eDoorPlaces.Left;
        }
        if (right)
        {
            doorPlaces |= Room.eDoorPlaces.Right;
        }

        room.DoorPlaces = doorPlaces;
    }

}
