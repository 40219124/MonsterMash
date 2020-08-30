using UnityEngine;

public class Agent : MonoBehaviour
{
	protected bool IsOurTurn { get; private set;}
	protected Agent Opponent { get; private set;}

	public Body Body;
	
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
		return IsOurTurn && battleController.TimeLeftOfAction <= 0;
	}
}