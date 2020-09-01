using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NumberRender : MonoBehaviour
{
	[SerializeField] List<SpriteRenderer> NumberSpriteRenderers;
	[SerializeField] List<Sprite> NumberSpriteList;
	
	public void SetNumber(int inputNumber)
	{
		int number = inputNumber;
		if (number < 0)
		{
			Debug.LogError($"NumberRender.SetNumber() call with {number} < 0", this);
			number *= -1;
		}
		

		for (int loop = NumberSpriteRenderers.Count-1; loop >= 0; loop--)
		{
			var spriteRenderer = NumberSpriteRenderers[loop];

			int reminder = number % 10;

			spriteRenderer.sprite = number == 0 ? null: NumberSpriteList[reminder];

			number = number/10;
		}

		if (number > 0)
		{
			Debug.LogError($"not enough NumberSpriteRenderers to display number: {inputNumber}", this);
		}
	}
}