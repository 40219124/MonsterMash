using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMonsterType { none = -1, Frankenstein, skeleton, mantis };

[System.Serializable]
public class MonsterProfile
{
    public BodyPartData Torso;
    public BodyPartData LeftArm;
    public BodyPartData RightArm;
    public BodyPartData Legs;

	public void HealToMax()
	{
		Debug.Log("Healing player to max");
		Torso.RestoreHealth();
		LeftArm.RestoreHealth();
		RightArm.RestoreHealth();
		Legs.RestoreHealth();
	}
}
