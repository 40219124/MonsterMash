using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleUiController: MonoBehaviour
{
	[SerializeField] Animator BattleUiAnimator;
	[SerializeField] DpadAnimator DpadAnimatorContoller;

	bool ForceShowComplexStats;

	void Update()
	{
		var battleController = BattleController.Instance;

		if (battleController.BattleState == BattleController.eBattleState.BattleIntro)
		{
			battleController.Player.Body.ShowStats(true, Body.eBodyPartType.None, false, isOurTurn:false, true);
			battleController.Enemy.Body.ShowStats(true, Body.eBodyPartType.None, false, isOurTurn:false, true);
			return;
		}


		var currentAgent = battleController.CurrentAgent;
		var opponent = currentAgent.Opponent;

		bool isPlayer = Settings.ShowStatsForAi || currentAgent.ControlType == Agent.eControlType.Player;

		if (SimpleInput.GetInputState(EInput.Select) == EButtonState.Pressed)
		{
			ForceShowComplexStats = !ForceShowComplexStats;
		}

		bool shouldPreShow = isPlayer && battleController.TimeLeftOfAction <= Settings.PreShowBattleUiTime;

		bool shouldPostShow = battleController.TimeSinceActionStarted <= Settings.PostPickHangTime &&
			battleController.CurrentAction != null;

		bool showUi = ForceShowComplexStats || ((shouldPreShow || shouldPostShow) && isPlayer);

		var attackerLocked = false;
		var targetLocked = false;
		var selectedAttacker = currentAgent.SelectedPart;
		var selectedTarget = Body.eBodyPartType.None;
		if (showUi)
		{
			if (shouldPostShow)
			{
				selectedAttacker = battleController.CurrentAction.AttackerPartType;
				selectedTarget = battleController.CurrentAction.TargetPartType;
				attackerLocked = true;
				targetLocked = true;
			}
			else
			{
				if (currentAgent.LockedAttacker != Body.eBodyPartType.None)
				{
					selectedAttacker = currentAgent.LockedAttacker;
					selectedTarget = currentAgent.SelectedPart;
					attackerLocked = true;
				}

				if (currentAgent.LockedTarget != Body.eBodyPartType.None)
				{
					selectedTarget = currentAgent.LockedTarget;
					targetLocked = true;
				}
			}
		}

		DpadAnimatorContoller.SetShow(showUi);
		if (showUi)
		{
			if (attackerLocked)
			{
				DpadAnimatorContoller.AnimateToPoint(opponent.Body.DPadGameTransform.position);
			}
			else
			{
				DpadAnimatorContoller.AnimateToPoint(currentAgent.Body.DPadGameTransform.position);
			}
		}

		currentAgent.Body.ShowStats(showUi, selectedAttacker, attackerLocked, isOurTurn:true, ForceShowComplexStats);
		opponent.Body.ShowStats(showUi, selectedTarget, targetLocked, isOurTurn:false, ForceShowComplexStats);
	}
}