using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
	const float CharactorsPerSecond = 20f;
	
	[SerializeField] TextMesh Text;
	[SerializeField] AudioClip TalkClip;
	[SerializeField] SpriteRenderer Background;

	string TargetString;
	float TimeAccumulator;

	void Start()
	{
		SetText("test string lets see if this work?", Vector2.zero);
	}

	public void SetText(string text, Vector2 postion)
	{
		TargetString = text;
		TimeAccumulator = 0;
		Text.text = "";
	}

	void Update()
	{
		if (Text.text.Length != TargetString.Length)
		{
			TimeAccumulator += Time.deltaTime;
			if (TimeAccumulator >= 1.0f/CharactorsPerSecond)
			{
				TimeAccumulator = 0;
				var nextCharactor = AddCharactor();

				if (nextCharactor == " ")
				{
					AddCharactor();
				}
				AudioSource.PlayClipAtPoint(TalkClip, transform.position);
			}
		}
	}

	string AddCharactor()
	{
		if (Text.text.Length == TargetString.Length)
		{
			return "";
		}

		var nextCharactor = TargetString.Substring(Text.text.Length, 1);
		Text.text += nextCharactor;
		return nextCharactor;
	}
}
