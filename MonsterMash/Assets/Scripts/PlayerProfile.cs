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
	NumberOfLevelsCompleted,
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
		public readonly string StatName;
		public float Value;
		float ScoreMultiplier;

		public StatValue(string statName, float scoreMultiplier=1, float value=0)
		{
			StatName = statName;
			Value = value;
			ScoreMultiplier = scoreMultiplier;
		}

		public float GetScore()
		{
			return Value * ScoreMultiplier;
		}
	}

	public PlayerProfile()
	{
		ProfileStats[eStatType.TurnsPlayedInBattle] = new StatValue("Turns Played");
		ProfileStats[eStatType.NumberOfBattlesPlayed] = new StatValue("Battles Played", scoreMultiplier:2);
		ProfileStats[eStatType.NumberOfRoomsCompleted] = new StatValue("Rooms Finished", scoreMultiplier:5);
		ProfileStats[eStatType.NumberOfLevelsCompleted] = new StatValue("Levels Finished", scoreMultiplier:20);
		ProfileStats[eStatType.NumberOfArmsSwapped] = new StatValue("Arms Swapped", scoreMultiplier:1);
		ProfileStats[eStatType.NumberOfLegsSwapped] = new StatValue("Legs Swapped", scoreMultiplier:1);
		ProfileStats[eStatType.NumberOfTorsoSwapped] = new StatValue("Torso Swapped", scoreMultiplier:3);
	}

	public int GetScore()
	{
		float score = 0;
		foreach (var kvp in ProfileStats)
		{
			score += (int)kvp.Value.GetScore();
		}
		return (int)score;
	}
}
