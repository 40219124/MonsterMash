public class Limb: BodyPart
{
	public int AttackTime { get { return PartData.AttackTimer; } }
	public int Damage { get { return PartData.Damage; } }

	public override void ShowStats(bool show, bool selected, bool isOurTurn, bool forceComplex)
	{
		bool shouldShow = show && IsAlive;

		float timeLeft = BattleController.Instance.TurnTimeLeft + Settings.ActionTimeForgiveness;
		if(BattleController.Instance.CurrentAction != null)
		{
			timeLeft -= BattleController.Instance.TimeLeftOfAction;
		}

		if (BattleController.Instance.BattleState == BattleController.eBattleState.TurnTransition)
		{
			timeLeft = Settings.TurnTime + BattleController.Instance.TurnTransitionTimeLeft;
		}


		bool disabled = !IsAlive || (timeLeft <= AttackTime && isOurTurn);

        StatBox.Show(shouldShow, selected, selected || forceComplex, disabled);


		StatBox.SetDamageNumber(show && (isOurTurn || selected || forceComplex), Damage);
		StatBox.SetTimeNumber(show && (isOurTurn || selected || forceComplex), AttackTime);

		StatBox.SetHealthNumber(show && (!isOurTurn || selected || forceComplex), CurrentHealth);
		StatBox.SetArmourNumber(show && (!isOurTurn || selected || forceComplex), Armour);
	}
}