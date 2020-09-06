using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour
{
	CollectableItem ItemData;
	bool roomHasEnemys;

	public void Setup(CollectableItem itemData, bool roomHasEnemys)
	{
		ItemData = itemData;
		transform.position = new Vector3(ItemData.Postion.x, ItemData.Postion.y, 0);
		gameObject.SetActive(ItemData.IsCollectAble && !roomHasEnemys);
	}

	public virtual bool PickUp(Vector3 playerPos)
	{
		var collected = gameObject.activeSelf && 
						transform.position == playerPos && 
						ItemData.IsCollectAble &&
						!roomHasEnemys;

		if(collected)
		{
			ItemData.PickUp();
			gameObject.SetActive(false);
		}

		return collected;
	}
}