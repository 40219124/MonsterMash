public class BodyPart
{
	public eBodyPartType PartType;
	public int Armour { get; private set;}
	public int MaxHealth { get; private set;}
	public int CurrentHealth { get; private set;}
	public bool IsAlive { get {return CurrentHealth > 0;}}

	public enum eBodyPartType
	{
		None,
		Arm,
		Leg,
		Torso,
	}
}