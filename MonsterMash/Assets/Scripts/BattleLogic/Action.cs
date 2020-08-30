public class Action
{
	public Limb AttackLimb { get; private set;}
	public BodyPart TargetBodyPart { get; private set;}
	
	public Action(Limb attackLimb, BodyPart targetBodayPart)
	{
		AttackLimb = attackLimb;
		TargetBodyPart = targetBodayPart;
	}

	public override string ToString()
	{
		return $"Attack: {AttackLimb}, Target: {TargetBodyPart}";
	}
}