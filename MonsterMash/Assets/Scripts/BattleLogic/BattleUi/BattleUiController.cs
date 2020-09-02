using UnityEngine;
using UnityEngine.UI;
using System;

public class BattleUiController: MonoBehaviour
{
	[SerializeField] Animator BattleUiAnimator;
	[SerializeField] Transform DPadTransform;

	void Awake()
	{
		DPadTransform.gameObject.SetActive(false);
	}

	void Update()
	{
		var battleController = BattleController.Instance;
		var currentAgent = battleController.CurrentAgent;
		var opponent = currentAgent.Opponent;

		bool isPlayer = currentAgent.ControlType == Agent.eControlType.Player;

		bool shouldPreShow = isPlayer && battleController.TimeLeftOfAction <= Settings.PreShowBattleUiTime;

		bool shouldPostShow = battleController.TimeSinceActionStarted <= Settings.PostPickHangTime &&
			battleController.CurrentAction != null;

		bool showUi = (shouldPreShow || shouldPostShow) && isPlayer;

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

		currentAgent.Body.ShowStats(showUi, selectedAttacker, attackerLocked, isOurTurn:true);
		opponent.Body.ShowStats(showUi, selectedTarget, targetLocked, isOurTurn:false);
	}
}