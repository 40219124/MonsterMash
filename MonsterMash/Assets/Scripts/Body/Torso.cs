public class Torso: BodyPart
{
	public override void ShowStats(bool show, bool selected)
	{
		bool shouldShow = show && IsAlive;
		StatBox.Show(shouldShow, selected, forceSimple:true);

		StatBox.SetHealthNumber(show, CurrentHealth);
		StatBox.SetArmourNumber(show, Armour);
	}
}