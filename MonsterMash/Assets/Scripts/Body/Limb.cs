public class Limb: BodyPart
{
	public int AttackTime;
	public int Damage;

	public override void ShowStats(bool show, bool selected)
	{
		base.ShowStats(show, selected);
		StatBox.SetDamageNumber(show, Damage);
		StatBox.SetTimeNumber(show, AttackTime);
		
		StatBox.SetHealthNumber(show, CurrentHealth);
		StatBox.SetArmourNumber(show, Armour);
	}
}