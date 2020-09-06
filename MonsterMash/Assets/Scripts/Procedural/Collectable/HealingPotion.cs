using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingPotion : CollectableItem
{
	public HealingPotion(Vector2Int pos): base(pos)
	{
		
	}

	public override void PickUp()
	{
		//todo heal the player
		base.PickUp();

	}
}