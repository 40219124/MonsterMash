public class Action
{
	public Body Attacker { get; private set;}
	public Body.eBodyPartType AttackerPartType { get; private set;}

	public Body Target { get; private set;}

	public Body.eBodyPartType TargetPartType { get; private set;}

	public Action(Body attacker, Body.eBodyPartType attackerPartType, Body target, Body.eBodyPartType targetPartType)
	{
		Attacker = attacker;
		AttackerPartType = attackerPartType;

		Target = target;
		TargetPartType = targetPartType;
	}

	public override string ToString()
	{
		return $"Attacker: {Attacker}, Target: {Target}";
	}
}