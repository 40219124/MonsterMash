using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerProfile: MonsterProfile
{
	public int NumberOfBattlesPlayed;
	public int NumberOfRoomsCompleted;
	public int NumberOfBodyPartsSwapped {get 
		{return NumberOfArmsSwapped + NumberOfLegsSwapped + NumberOfTorsoSwapped;}
	}
	public int NumberOfArmsSwapped;
	public int NumberOfLegsSwapped;
	public int NumberOfTorsoSwapped;
	public int NumberOfLimbsLost;

	public int NumberOfHealingPotionsUsed;

	public int TotalDamageDealt;
	public int TotalDamageReceived;
	public int TotalIncomingDamageBlocked;
	public int TotalOutGoingDamageBlocked;
	public int TotalHealthHealed;
}
