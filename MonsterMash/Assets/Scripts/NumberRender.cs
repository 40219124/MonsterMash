using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class NumberRender : MonoBehaviour
{
	[SerializeField] List<SpriteRenderer> NumberSpriteRenderers;
	[SerializeField] List<Sprite> NumberSpriteList;

	[SerializeField] List<Sprite> LargeNumberSpriteList;

	[SerializeField] Animator NumberAnimator;
	

	public bool UseLargeNumbers;
	
	void Update()
	{
		NumberAnimator.SetBool("Small", !UseLargeNumbers);
	}

	public void SetNumber(int inputNumber)
	{
		
		int number = inputNumber;
		if (number < 0)
		{
			MMLogger.LogError($"NumberRender.SetNumber() call with {number} < 0", this);
			number *= -1;
		}
		

		for (int loop = NumberSpriteRenderers.Count-1; loop >= 0; loop--)
		{
			var spriteRenderer = NumberSpriteRenderers[loop];

			int reminder = number % 10;

			spriteRenderer.sprite = number == 0 && loop != NumberSpriteRenderers.Count-1 ? null: GetNumberSprite(reminder);

			number = number/10;
		}

		if (number > 0)
		{
			MMLogger.LogError($"not enough NumberSpriteRenderers to display number: {inputNumber}", this);
		}
	}

	Sprite GetNumberSprite(int singleDigitNumber)
	{
		if (singleDigitNumber < 0)
		{
			MMLogger.LogError($"GetNumberSprite() call with {singleDigitNumber} < 0", this);
			return null;
		}
		if (singleDigitNumber/10 != 0)
		{
			MMLogger.LogError($"GetNumberSprite() call with {singleDigitNumber} not singleDigitNumber", this);
			return null;
		}

		var spriteList = NumberSpriteList;

		if(UseLargeNumbers)
		{
			spriteList = LargeNumberSpriteList;
		}

		return spriteList[singleDigitNumber];
	}
}