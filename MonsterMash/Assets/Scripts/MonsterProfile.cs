using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMonsterType { none = -1, Frankenstein, skeleton };

[System.Serializable]
public class MonsterProfile
{
    public BodyPartData Torso;
    public BodyPartData LeftArm;
    public BodyPartData RightArm;
    public BodyPartData Legs;
}
