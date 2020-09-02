using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class RoomEditor : EditorWindow
{
    static AllRoomData AllRooms;
    static string jsonFile = "/RoomData.json";
    Vector2 ScrollPos;

    string theText = "numnuts";

    [MenuItem("MonsterMash/Room Editor")]
    static void Init()
    {
        RoomEditor window = (RoomEditor)EditorWindow.GetWindow(typeof(RoomEditor));

        LoadRooms();

        window.Show();
    }
    private void OnGUI()
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

                if (GUILayout.Button("Delete Room"))
                {
                    AllRooms.AllRooms.Remove(room);
                }
            }
        }

        EditorGUILayout.EndScrollView();
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
        GUILayout.BeginHorizontal();

        GUILayout.Label("Room Id", EditorStyles.boldLabel);
        GUILayout.Label($"{room.RoomId}");
        GUILayout.Space(1000);

        GUILayout.EndHorizontal();

        room.IsBossRoom = EditorGUILayout.Toggle("Boss Room", room.IsBossRoom);

        GUILayout.BeginHorizontal();
        
        bool top =EditorGUILayout.Toggle("Top", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Top));
        bool botttom = EditorGUILayout.Toggle("Bottom", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Bottom));
        GUILayout.Space(700);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        bool left = EditorGUILayout.Toggle("Left", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Left));
        bool right = EditorGUILayout.Toggle("Right", room.DoorPlaces.HasFlag(Room.eDoorPlaces.Right));
        GUILayout.Space(700);
        GUILayout.EndHorizontal();

        SetRoomDoors(ref room, top, botttom, left, right);

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
