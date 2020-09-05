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
    static MonsterProfile opponentLoot;

    // Profiles
    public static void RecordProfile(MonsterProfile profile, int id = -1)
    {
        // Set player profile
        if (id == -1)
        {
            PlayerProfile = profile;
            return;
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

    public static MonsterProfile GetEnemyProfile(int id)
    {
        return EnemyProfiles[id];
    }

    public static Dictionary<int, MonsterProfile> GetEnemyProfiles()
    {
        return EnemyProfiles;
    }

    // Get profiles for combat
    public static MonsterProfile GetCombatProfile(bool isPlayer)
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

    // Positions
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

    public static Vector3 GetEnemyPosition(int id)
    {
        return EnemyPositions[id];
    }

    public static Dictionary<int, Vector3> GetEnemyPositions()
    {
        return EnemyPositions;
    }

    // Update enemy id's after re-creation
    public static void UpdateID(int oldID, int newID)
    {
        // Change key position
        EnemyPositions.Add(newID, EnemyPositions[oldID]);
        EnemyPositions.Remove(oldID);

        // Change key profile
        EnemyProfiles.Add(newID, EnemyProfiles[oldID]);
        EnemyProfiles.Remove(oldID);
    }


    // Player position
    public static Vector3 GetPlayerPosition()
    {
        return PlayerPos;
    }

    // Opponent for combat
    public static int OpponentID
    {
        get { return opponentID; }
        set { opponentID = value; }
    }

    public static void OpponentBeaten(bool getLoot)
    {
        EnemyPositions.Remove(OpponentID);
        if (getLoot)
        {
            opponentLoot = EnemyProfiles[opponentID];
        }
        EnemyProfiles.Remove(OpponentID);
    }

    public static MonsterProfile GetLootProfile()
    {
        return opponentLoot;
    }

    public static void ClearLoot()
    {
        opponentLoot = null;
    }

    // To clear info // ~~~ change once spawning supplies id's instead of distruction
    public static void ClearEnemies()
    {
        EnemyPositions.Clear();
        EnemyProfiles.Clear();
        opponentID = 0;
    }
}
