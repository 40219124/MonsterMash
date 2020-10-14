public class Limb: BodyPart
{
	public int AttackTime { get { return PartData == null? 0 : PartData.AttackTimer; } }
	public int Damage { get { return PartData == null? 0 : PartData.Damage; } }

	public override void ShowStats(bool show, bool selected, bool isOurTurn, bool forceComplex, bool forceDisable=false)
	{
		bool shouldShow = show && IsAlive;



		bool disabled = forceDisable || !IsAlive || (!IsValidAttacker() && !Agent.AbleToPreQueue() && isOurTurn);

        StatBox.Show(shouldShow, selected, selected || forceComplex, disabled);


		StatBox.SetDamageNumber(show && (isOurTurn || selected || forceComplex), Damage);
		StatBox.SetTimeNumber(show && (isOurTurn || selected || forceComplex), AttackTime);

		StatBox.SetHealthNumber(show && (!isOurTurn || selected || forceComplex), CurrentHealth);
		StatBox.SetArmourNumber(show && (!isOurTurn || selected || forceComplex), Armour);
	}

	public bool IsValidAttacker()
	{
		if (BattleController.Instance == null)
		{
			return true;
		}
		float timeLeft = BattleController.Instance.TurnTimeLeft + Settings.ActionTimeForgiveness;
		if(BattleController.Instance.CurrentAction != null)
		{
			timeLeft -= BattleController.Instance.TimeLeftOfAction;
		}

		if (BattleController.Instance.BattleState == BattleController.eBattleState.TurnTransition)
		{
			timeLeft = Settings.TurnTime + BattleController.Instance.TurnTransitionTimeLeft;
		}
		return IsAlive && timeLeft >= AttackTime;
	}
}