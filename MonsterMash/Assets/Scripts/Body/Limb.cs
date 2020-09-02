public class Limb: BodyPart
{
	public int AttackTime { get { return PartData.AttackTimer; } }
	public int Damage { get { return PartData.Damage; } }

	public override void ShowStats(bool show, bool selected, bool isOurTurn, bool forceComplex)
	{
		bool shouldShow = show && IsAlive;

		bool disabled = !IsAlive || 
			((BattleController.Instance.TurnTimeLeft + Settings.ActionTimeForgiveness <= AttackTime &&
			BattleController.Instance.BattleState != BattleController.eBattleState.TurnTransition) &&
			isOurTurn);

        StatBox.Show(shouldShow, selected, selected || forceComplex, disabled);


		StatBox.SetDamageNumber(show && (isOurTurn || selected || forceComplex), Damage);
		StatBox.SetTimeNumber(show && (isOurTurn || selected || forceComplex), AttackTime);

		StatBox.SetHealthNumber(show && (!isOurTurn || selected || forceComplex), CurrentHealth);
		StatBox.SetArmourNumber(show && (!isOurTurn || selected || forceComplex), Armour);
	}
}