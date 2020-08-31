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
	
	public Body.eBodyPartType SelectedPart { get; private set;}

	public Body.eBodyPartType LockedAttacker { get; private set;}
	public Body.eBodyPartType LockedTarget { get; private set;}

	void Update()
	{
		if (!CanDoAction())
		{
			return;
		}


		if (ControlType == eControlType.Player)
		{
			float horizontalValue = Input.GetAxisRaw("Horizontal");
			float verticalValue = Input.GetAxisRaw("Vertical");
			float aButton = Input.GetAxisRaw("ButtonA");

			if (horizontalValue > 0 && verticalValue == 0)
			{
				SelectedPart = Body.eBodyPartType.RightArm;
			}
			else if (horizontalValue < 0 && verticalValue == 0)
			{
				SelectedPart = Body.eBodyPartType.LeftArm;
			}
			else if (horizontalValue == 0 && verticalValue < 0)
			{
				SelectedPart = Body.eBodyPartType.Leg;
			}
			else if (horizontalValue == 0 && horizontalValue < 0 && LockedAttacker != Body.eBodyPartType.None)
			{
				SelectedPart = Body.eBodyPartType.Torso;
			}

			if (aButton > 0)
			{
				if (LockedAttacker == Body.eBodyPartType.None)
				{
					LockedAttacker = SelectedPart;
				}
				else
				{
					LockedTarget = SelectedPart;
				}
				SelectedPart = Body.eBodyPartType.None;
			}
		}
		else if (ControlType == eControlType.Ai)
		{
			LockedAttacker = Body.eBodyPartType.LeftArm;
			LockedTarget = Body.eBodyPartType.Torso;
		}
		
		if (LockedAttacker != Body.eBodyPartType.None &&
			LockedTarget != Body.eBodyPartType.None)
		{
			var action = new Action(Body, LockedAttacker, Opponent.Body, LockedTarget);
			if (BattleController.Instance.TryAction(action))
			{
				LockedAttacker = Body.eBodyPartType.None;
				LockedTarget = Body.eBodyPartType.None;
			}
		}
	}
	
	public void OnGameStart(Agent opponent)
	{
		Opponent = opponent;
	}

	public void OnTurnStart(bool isOurTurn)
	{
		IsOurTurn = isOurTurn;
		LockedAttacker = Body.eBodyPartType.None;
		LockedTarget = Body.eBodyPartType.None;
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