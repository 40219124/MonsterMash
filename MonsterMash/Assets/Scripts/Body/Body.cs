using UnityEngine;

public class Body : MonoBehaviour
{
	public Torso TorsoPart;
	public Limb LeftArmPart;
	public Limb RightArmPart;
	public Limb LegsPart;

	public enum eBodyPartType
	{
		None,
		LeftArm,
		RightArm,
		Leg,
		Torso,
	}

	[Space]
	[Header("UI")]
	[SerializeField] Animator BodyAnimator;

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

	public void StartAttack()
	{
		if (BodyAnimator != null)
		{
			BodyAnimator.SetBool("Doing Attack", true);
		}
	}

	public void EndAttack()
	{
		if (BodyAnimator != null)
		{
			BodyAnimator.SetBool("Doing Attack", false);
		}
	}

	public void ApplyAttack(eBodyPartType targetBodyPartType, int damage)
	{
		if (BodyAnimator != null)
		{
			BodyAnimator.SetTrigger("Been Hit");
		}

		var targetPart = GetBodyPart(targetBodyPartType);
		targetPart.ApplyAttack(damage);
	}

	public Limb GetLimb(eBodyPartType targetBodyPartType)
	{
		switch (targetBodyPartType)
		{
			case eBodyPartType.LeftArm:
			{
				return LeftArmPart;
			}
			case eBodyPartType.RightArm:
			{
				return RightArmPart;
			}
			case eBodyPartType.Leg:
			{
				return LegsPart;
			}
			default:
			{
				return null;
			}
		}
	}
	public BodyPart GetBodyPart(eBodyPartType targetBodyPartType)
	{
		BodyPart bodyPart = null;
		if (targetBodyPartType == eBodyPartType.Torso)
		{
			bodyPart = TorsoPart;
		}
		else
		{
			bodyPart = GetLimb(targetBodyPartType);
		}
		return bodyPart;
	}
}