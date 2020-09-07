using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPrize : CollectableItem
{
	public BossPrize(Vector2Int pos): base(pos)
	{
		
	}

	public override void PickUp()
	{
		base.PickUp();

		if (ProceduralDungeon.Instance.NumberOfDungeonsMade == 1)
		{
			ProceduralDungeon.Instance.GenerateMap(Room.eArea.Indoors);
			FlowManager.Instance.TransToOverworld(Settings.SceneOverworld);
		}
		else
		{
			FlowManager.Instance.TransToGameOver(Settings.SceneOverworld, true);
		}
	}
}