using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpadAnimator : MonoBehaviour
{
	const float Speed = 25f; //UnitsPerSecond

	[SerializeField] Transform Root;
	[SerializeField] Animator Animator;

	[SerializeField] GameObject ArrowIconUp;
	[SerializeField] GameObject ArrowIconRight;
	[SerializeField] GameObject ArrowIconDown;
	[SerializeField] GameObject ArrowIconLeft;

	float AnimationTime = 1;
	float TimeAccumulator = 1;

	Vector3 StartPos;
	Vector3 EndPos;

	void Awake()
	{
		TimeAccumulator = AnimationTime;
		StartPos = Root.position;
		EndPos = Root.position;
	}

	public void JumpToPoint(Vector3 pos)
	{
		TimeAccumulator = AnimationTime;
		EndPos = pos;
	}

	public void AnimateToPoint(Vector3 pos)
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
		if (gameObject.active)
		{
			TimeAccumulator += Time.deltaTime;
			TimeAccumulator = Mathf.Min(TimeAccumulator, AnimationTime);
			
			float progress = TimeAccumulator/AnimationTime;
			float newX = StartPos.x + ( EndPos.x - StartPos.x ) * progress;
			float newY = StartPos.y + ( EndPos.y - StartPos.y ) * progress;
			Root.position = new Vector3(newX, newY, Root.position.z);
		}
		else
		{
			Root.position = EndPos;
		}
		ArrowIconUp.SetActive(SimpleInput.GetInputActive(EInput.dpadUp));
		ArrowIconRight.SetActive(SimpleInput.GetInputActive(EInput.dpadRight));
		ArrowIconDown.SetActive(SimpleInput.GetInputActive(EInput.dpadDown));
		ArrowIconLeft.SetActive(SimpleInput.GetInputActive(EInput.dpadLeft));
	}

	public void SetShow(bool show)
	{
		gameObject.SetActive(show);
	}
}
