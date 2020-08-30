using UnityEngine;
using System;

public class DebugBattleStarter : MonoBehaviour
{
	public Agent Player;
	public Agent Enemy;

	void Start()
	{
		BattleController.Instance.SetupBattle(Player, Enemy);
	}
}