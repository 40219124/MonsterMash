public class Torso: BodyPart
{
	public override void ShowStats(bool show, bool selected)
	{
		base.ShowStats(show, selected);

		StatBox.SetHealthNumber(show, CurrentHealth);
		StatBox.SetArmourNumber(show, Armour);
	}
}