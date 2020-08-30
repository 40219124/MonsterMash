using UnityEngine;

public class AiAgent: Agent
{
	void Update()
	{
		if (CanDoAction())
		{
			var attackLimb = Opponent.Body.LeftArmPart;
			var targetBodyPart = Opponent.Body.TorsoPart;
			var action = new Action(attackLimb, targetBodyPart);
			BattleController.Instance.TryAction(action);
		}
	}
}