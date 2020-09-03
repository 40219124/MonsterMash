using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpadAnimator : MonoBehaviour
{
	const float Speed = 15f; //UnitsPerSecond

	[SerializeField] Transform Root;
	[SerializeField] Animator Animator;

	float AnimationTime;
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

	public void AnimateToPoint(Vector2 pos)
	{
		if (pos != EndPos)
		{
			TimeAccumulator = 0;
			StartPos = EndPos;
			EndPos = pos;
			var distance = (EndPos-StartPos).magnitude;
			AnimationTime = distance/Speed;
		}
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
