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

		if (ControlType == eControlType.Player)
		{
			
			
		}
		else if (ControlType == eControlType.Ai)
		{
			var attackLimb = Opponent.Body.LeftArmPart;
			var targetBodyPart = Opponent.Body.TorsoPart;
			var action = new Action(attackLimb, targetBodyPart);
			BattleController.Instance.TryAction(action);
		}
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