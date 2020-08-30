using UnityEngine;
using System;

public class DebugBattleStarter : MonoBehaviour
{

	[SerializeField] Agent Player;
	[SerializeField] Agent Enemy;

	[SerializeField] bool WillLoopBattles;

	void Start()
	{
		BattleController.Instance.SetupBattle(Player, Enemy);
	}

	void Update()
	{
		if (WillLoopBattles)
		{
			var battleState = BattleController.Instance.BattleState;
			if (battleState == BattleController.eBattleState.PlayerWon ||
				battleState == BattleController.eBattleState.EnemyWon)
			{
				BattleController.Instance.SetupBattle(Player, Enemy);
			}
		}
	}
}