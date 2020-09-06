using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem
{
	public Vector2Int Postion;
	public bool IsCollectAble = true;

	public CollectableItem()
	{
		//todo make this random
		Postion = new Vector2Int(5,5);
	}

	public virtual void PickUp()
	{
		IsCollectAble = false;
	}
}