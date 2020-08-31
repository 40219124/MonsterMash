using UnityEngine;

public class Agent : MonoBehaviour
{
	protected bool IsOurTurn { get; private set;}
	protected Agent Opponent { get; private set;}

	public Body Body;
	
	public eControlType ControlType;
	public enum eControlType
	{
		None,
		Player,
		Ai
	}

	void Update()
	{
		if (!CanDoAction())
		{
			return;
		}

		var attackerLimbType = Body.eBodyPartType.None;
		var targetBodyPartType = Body.eBodyPartType.None;

		if (ControlType == eControlType.Player)
		{
			
			
		}
		else if (ControlType == eControlType.Ai)
		{
			attackerLimbType = Body.eBodyPartType.LeftArm;
			targetBodyPartType = Body.eBodyPartType.Torso;
		}

		var action = new Action(Body, attackerLimbType, Opponent.Body, targetBodyPartType);
		BattleController.Instance.TryAction(action);
	}
	
	public void OnGameStart(Agent opponent)
	{
		Opponent = opponent;
	}

	public void OnTurnStart(bool isOurTurn)
	{
		IsOurTurn = isOurTurn;
	}


	protected bool CanDoAction()
	{
		var battleController = BattleController.Instance;
		return IsOurTurn && 
			battleController.TimeLeftOfAction <= 0 &&
			(battleController.BattleState == BattleController.eBattleState.PlayerTurn ||
			battleController.BattleState == BattleController.eBattleState.EnemyTurn);
	}
}