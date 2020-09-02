public class Torso: BodyPart
{
	public override void ShowStats(bool show, bool selected, bool isOurTurn)
	{
		bool shouldShow = show && IsAlive;
		StatBox.Show(shouldShow, selected, showComplex:false, isOurTurn);

		StatBox.SetHealthNumber(show, CurrentHealth);
		StatBox.SetArmourNumber(show, Armour);
	}
}