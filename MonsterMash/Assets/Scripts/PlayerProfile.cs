using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum eStatType
{
	TimeProfileCreated,
	TimePlayed,
	TurnsPlayedInBattle,
	NumberOfBattlesPlayed,
	NumberOfRoomsCompleted,
	NumberOfArmsSwapped,
	NumberOfLegsSwapped,
	NumberOfTorsoSwapped,
	NumberOfLimbsLost,
	NumberOfHealingPotionsUsed,
	TotalDamageDealt,
	TotalDamageReceived,
	TotalIncomingDamageBlocked,
	TotalOutGoingDamageBlocked,
	TotalHealthHealed,
	DistanceWalkedInOverWorld,
}

[System.Serializable]
public class PlayerProfile
{
	public MonsterProfile CombatProfile;

	public Dictionary<eStatType, StatValue> ProfileStats = new Dictionary<eStatType, StatValue>();

	public class StatValue
	{
		public string StatName;
		public float Value;

		public StatValue(string statName, float value=0)
		{
			StatName = statName;
			Value = value;
		}
	}

	public PlayerProfile()
	{
		ProfileStats[eStatType.TurnsPlayedInBattle] = new StatValue("Turns Played");
		ProfileStats[eStatType.NumberOfBattlesPlayed] = new StatValue("Battles Played");
		ProfileStats[eStatType.NumberOfRoomsCompleted] = new StatValue("Rooms Finished");
		ProfileStats[eStatType.NumberOfArmsSwapped] = new StatValue("Arms Swapped");
		ProfileStats[eStatType.NumberOfLegsSwapped] = new StatValue("Legs Swapped");
		ProfileStats[eStatType.NumberOfTorsoSwapped] = new StatValue("Torso Swapped");
	}
}
