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
		//todo win game
		base.PickUp();
	}
}