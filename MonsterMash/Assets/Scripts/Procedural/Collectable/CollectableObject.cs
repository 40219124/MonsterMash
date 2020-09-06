using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObject : MonoBehaviour
{
	CollectableItem ItemData;

	public void Setup(CollectableItem itemData)
	{
		ItemData = itemData;
		transform.position = new Vector3(ItemData.Postion.x, ItemData.Postion.y, 0);
		gameObject.SetActive(ItemData.IsCollectAble);
	}

	public virtual bool PickUp(Vector3 playerPos)
	{
		var collected = gameObject.activeSelf && 
						transform.position == playerPos && 
						ItemData.IsCollectAble;

		if(collected)
		{
			ItemData.PickUp();
			gameObject.SetActive(false);
		}

		return collected;
	}
}