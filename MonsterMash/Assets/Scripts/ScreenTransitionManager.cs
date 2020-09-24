using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTransitionManager : AdditiveSceneManager
{
	[SerializeField] Animator TransitionAnimator;
	[SerializeField] Transform RootTransform;

	public static ScreenTransitionManager Instance;

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
		MMLogger.Log($"setting show black to: {showBlack} was {Instance.ShowingBlack}");
		Instance.ShowingBlack = showBlack;

		var rootPos = Vector3.zero;
		if (pos != null)
		{
			rootPos = new Vector3(pos.x, pos.y);
		}

		Instance.RootTransform.position = rootPos;
	}

	// void Update()
	// {
	// 	if (SimpleInput.GetInputState(EInput.A) == EButtonState.Released)
	// 	{
	// 		SetShowBlack(!ShowingBlack);
	// 	}

	// }

	public static IEnumerator WaitForSetBlack(bool showBlack, Vector2 pos)
	{
		SetShowBlack(showBlack, pos);

		AnimatorStateInfo stateInfo;
		do
		{
			yield return null;
			stateInfo = Instance.TransitionAnimator.GetCurrentAnimatorStateInfo(0);
		} while (stateInfo.IsName("To White") || stateInfo.IsName("To Black"));

		yield break;
	}
}
