using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerProfile
{
	public MonsterProfile CombatProfile;

	public float TimeProfileCreated;
	public float TimePlayed;

	public float TurnsPlayedInBattle;
	public int NumberOfBattlesPlayed;
	
	public int NumberOfRoomsCompleted;


	public int NumberOfArmsSwapped;
	public int NumberOfLegsSwapped;
	public int NumberOfTorsoSwapped;
	public int NumberOfBodyPartsSwapped {get 
		{return NumberOfArmsSwapped + NumberOfLegsSwapped + NumberOfTorsoSwapped;}
	}

	public int NumberOfLimbsLost;
	public int NumberOfHealingPotionsUsed;

	public int TotalDamageDealt;
	public int TotalDamageReceived;
	public int TotalIncomingDamageBlocked;
	public int TotalOutGoingDamageBlocked;
	public int TotalHealthHealed;

	public float DistanceWalkedInOverWorld;
}
