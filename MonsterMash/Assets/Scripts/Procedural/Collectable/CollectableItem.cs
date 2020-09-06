using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem
{
	public Vector2Int Postion;
	public bool IsCollectAble = true;

	public CollectableItem(Vector2Int pos)
	{
		Postion = pos;
	}

	public virtual void PickUp()
	{
		IsCollectAble = false;
	}
}