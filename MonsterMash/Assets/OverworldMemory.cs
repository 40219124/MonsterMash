using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class OverworldMemory : MonoBehaviour
{

    static MonsterProfile PlayerProfile;
    static Dictionary<int, MonsterProfile> EnemyProfiles = new Dictionary<int, MonsterProfile>();

    static Vector3 PlayerPos;
    static Dictionary<int, Vector3> EnemyPositions = new Dictionary<int, Vector3>();

    static int opponentID;

    public static void RecordProfile(MonsterProfile profile, int id = -1)
    {
        // Set player profile
        if (id == -1)
        {
            PlayerProfile = profile;
        }


        // Set enemy profile
        if (!EnemyProfiles.ContainsKey(id))
        {
            EnemyProfiles.Add(id, profile);
        }
        else
        {
            EnemyProfiles[id] = profile;
        }
    }

    public static void RecordPosition(Vector3 pos, int id = -1)
    {
        // Save player pos
        if (id == -1)
        {
            PlayerPos = pos;
            return;
        }


        // Save enemy pos
        if (!EnemyPositions.ContainsKey(id))
        {
            EnemyPositions.Add(id, pos);
        }
        else
        {
            EnemyPositions[id] = pos;
        }
    }

    public static int OpponentID
    {
        get { return opponentID; }
        set { opponentID = value; }
    }

    public static MonsterProfile GetProfile(bool isPlayer)
    {
        if (isPlayer)
        {
            return PlayerProfile;
        }
        else
        {
            return EnemyProfiles[opponentID];
        }
    }
}
