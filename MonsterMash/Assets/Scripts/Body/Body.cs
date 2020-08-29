public class Body
{
	public Torso TorsoPart { get; private set;}
	public Limb LeftArmPart { get; private set;}
	public Limb RightArmPart { get; private set;}
	public Limb LegsPart { get; private set;}

	public bool IsAlive()
	{
		return TorsoPart.IsAlive &&
			(LeftArmPart.IsAlive ||
			RightArmPart.IsAlive ||
			LegsPart.IsAlive);
	}
}