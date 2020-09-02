using UnityEngine;
using System;

public class DebugBattleStarter : MonoBehaviour
{
	void Start()
	{
		BattleController.Instance.SetupBattle();
	}
}