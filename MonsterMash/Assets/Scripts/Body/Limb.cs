public class Limb: BodyPart
{
	public int AttackTime;
	public int Damage;

	public override void ShowStats(bool show, bool selected, bool isOurTurn)
	{
		base.ShowStats(show, selected, isOurTurn);


		StatBox.SetDamageNumber(show && (isOurTurn || selected), Damage);
		StatBox.SetTimeNumber(show && (isOurTurn || selected), AttackTime);

		StatBox.SetHealthNumber(show && (!isOurTurn || selected), CurrentHealth);
		StatBox.SetArmourNumber(show && (!isOurTurn || selected), Armour);
	}
}