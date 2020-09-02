public class Limb: BodyPart
{
	public int AttackTime { get { return PartData.AttackTimer; } }
	public int Damage { get { return PartData.Damage; } }

	public override void ShowStats(bool show, bool selected, bool isOurTurn)
	{
		bool shouldShow = show && IsAlive;

		bool disabled = !IsAlive || 
			(BattleController.Instance.TurnTimeLeft + Settings.ActionTimeForgiveness <= AttackTime &&
			isOurTurn);

        StatBox.Show(shouldShow, selected, selected, disabled);


		StatBox.SetDamageNumber(show && (isOurTurn || selected), Damage);
		StatBox.SetTimeNumber(show && (isOurTurn || selected), AttackTime);

		StatBox.SetHealthNumber(show && (!isOurTurn || selected), CurrentHealth);
		StatBox.SetArmourNumber(show && (!isOurTurn || selected), Armour);
	}
}