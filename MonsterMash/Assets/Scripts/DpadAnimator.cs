using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpadAnimator : MonoBehaviour
{
	const float AnimationTime = 1;

	[SerializeField] Transform Root;

	float TimeAccumulator;

	Vector2 StartPos;
	Vector2 EndPos;

	void Awake()
	{
		TimeAccumulator = AnimationTime;
		SetShow(false);
	}

	public void JumpToPoint(Vector2 pos)
	{
		TimeAccumulator = AnimationTime;
		EndPos = pos;
	}

    public void AnimateBetweenPoints(Vector2 startPos, Vector2 endPos)
	{
		TimeAccumulator = 0;
		StartPos = startPos;
		EndPos = endPos;
	}

	void Update()
	{
		TimeAccumulator += Time.deltaTime;
		TimeAccumulator = Mathf.Min(TimeAccumulator, AnimationTime);
		
		float progress = TimeAccumulator/AnimationTime;
		float newX = StartPos.x + ( EndPos.x - StartPos.x ) * progress;
		float newY = StartPos.y + ( EndPos.y - StartPos.y ) * progress;
		Root.position = new Vector3(newX, newY, Root.position.z);
	}

	public void SetShow(bool show)
	{
		gameObject.SetActive(show);
	}
}
