using UnityEngine;
using System;

public class DebugBattleStarter : MonoBehaviour
{
	[SerializeField]
	MonsterProfile Player;
	[SerializeField]
	MonsterProfile Enemy;
	void Start()
	{
		BattleController.Instance.SetupBattle(Player, Enemy);
	}
}