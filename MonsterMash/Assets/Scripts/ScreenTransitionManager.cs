using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionManager : AdditiveSceneManager
{
	[SerializeField] Animator TransitionAnimator;
	[SerializeField] Transform RootTransform;

	public static ScreenTransitionManager Instance;
	
	public static bool IsAnimating
	{get
		{
			if (Instance == null)
			{
				return false;
			}
			var stateInfo = Instance.TransitionAnimator.GetCurrentAnimatorStateInfo(0);
			return stateInfo.IsName("To White") || stateInfo.IsName("To Black");
		}
	}

	bool ShowingBlack;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			MMLogger.LogError("ScreenTransitionManager.Instance already setup");
		}
	}

	public static void SetShowBlack(bool showBlack, Vector2 pos)
	{
		Instance.TransitionAnimator.SetBool("ShowBlack", showBlack);
		MMLogger.Log($"setting show black to: {showBlack} was {Instance.ShowingBlack} at pos: {pos}");
		Instance.ShowingBlack = showBlack;

		Instance.RootTransform.position = new Vector3(pos.x, pos.y);

		Instance.TransitionAnimator.speed = 2;
	}

	public static IEnumerator WaitForSetBlack(bool showBlack, Vector2 pos)
	{
		SetShowBlack(showBlack, pos);
		yield return null;

		do
		{
			yield return null;
		} while (IsAnimating);

		yield break;
	}
}
