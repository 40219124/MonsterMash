using UnityEngine;

public class Body : MonoBehaviour
{
	public Torso TorsoPart;
	public Limb LeftArmPart;
	public Limb RightArmPart;
	public Limb LegsPart;

	public bool IsAlive()
	{
		return TorsoPart.IsAlive &&
			(LeftArmPart.IsAlive ||
			RightArmPart.IsAlive ||
			LegsPart.IsAlive);
	}

	public bool IsOurBodyPart(BodyPart bodyPart)
	{
		return TorsoPart == bodyPart ||
			   LeftArmPart == bodyPart ||
			   RightArmPart == bodyPart ||
			   LegsPart == bodyPart;
	}

	public override string ToString()
	{
		return $"Torso: {TorsoPart}, LeftArm: {LeftArmPart}, RightArm: {RightArmPart}, Legs: {LegsPart}";
	}
}