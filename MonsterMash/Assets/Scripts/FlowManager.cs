using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OverworldMemory))]
public class FlowManager : MonoBehaviour
{
	public static FlowManager Instance;
	List<string> LoadingScreens = new List<string>();

	void Start()
	{
		TransToOverworld();
		Instance = this;
	}
	public void TransOverworldToBattle()
	{
		StartCoroutine(TransOverworldToBattleCo());
	}

	IEnumerator TransOverworldToBattleCo()
	{
		if (IsScreenLoading(Settings.SceneBattle))
		{
			MMLogger.LogWarning($"TransOverworldToBattleCo called but screen already loading so stopping call");
			yield break;
		}

		AddScreenToLoading(Settings.SceneBattle);

		var player = FindObjectOfType<Player>();
		yield return ScreenTransitionManager.WaitForSetBlack(true, player.transform.position);

		yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(Settings.SceneOverworld));
		yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.SceneBattle));
		yield return null;
		BattleController bc = FindObjectOfType<BattleController>();
		if (bc != null)
		{
			bc.SetupBattle(OverworldMemory.GetCombatProfile(true), OverworldMemory.GetCombatProfile(false));
		}

		ScreenTransitionManager.SetShowBlack(false, Vector2.zero);

		RemoveScreenFromLoading(Settings.SceneBattle);
	}


	// transition to overworld from unknown scene
	public void TransToOverworld(string sceneFrom = "", bool isNewDungeon=false)
	{
		StartCoroutine(TransToOverworldCo(sceneFrom, isNewDungeon));
	}

	IEnumerator TransToOverworldCo(string sceneFrom, bool isNewDungeon)
	{
		if (IsScreenLoading(Settings.SceneOverworld))
		{
			MMLogger.LogWarning($"TransToOverworldCo called but screen already loading so stopping call");
			yield break;
		}

		AddScreenToLoading(Settings.SceneOverworld);

		MMLogger.Log($"TransToOverworldCo called with sceneFrom:{sceneFrom}, isNewDungeon:{isNewDungeon}");
		if (!sceneFrom.Equals(""))
		{
			if (sceneFrom != Settings.SceneOverworld)
			{
				yield return ScreenTransitionManager.WaitForSetBlack(true, Vector2.zero);
			}
			yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(sceneFrom));
			
		}

		yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.SceneOverworld));
		FindObjectOfType<CurrentRoom>().Setup(sceneFrom == Settings.SceneOverworld && !isNewDungeon);
		yield return null;
		var transPos = Vector2.zero;
		var player = FindObjectOfType<Player>();
		if (player != null)
		{
			transPos = player.transform.position;
		}
		ScreenTransitionManager.SetShowBlack(false, transPos);

		RemoveScreenFromLoading(Settings.SceneOverworld);
	}

	public void TransToGameOver(string sceneFrom, bool wonTheGame)
	{
		StartCoroutine(TransToGameOverCo(sceneFrom, wonTheGame));
	}

	IEnumerator TransToGameOverCo(string sceneFrom, bool wonTheGame)
	{
		if (IsScreenLoading(Settings.SceneGameOver))
		{
			MMLogger.LogWarning($"TransToGameOverCo called but screen already loading so stopping call");
			yield break;
		}

		AddScreenToLoading(Settings.SceneGameOver);

		var transPos = Vector2.zero;
		var player = FindObjectOfType<Player>();
		if (player != null)
		{
			transPos = player.transform.position;
		}
		yield return ScreenTransitionManager.WaitForSetBlack(true, transPos);

		if (!sceneFrom.Equals(""))
		{
			yield return MainManager.Instance.SubtractSceneCo(sceneFrom);
		}
		yield return MainManager.Instance.AddSceneCo(Settings.SceneGameOver);
		yield return null;
		FindObjectOfType<GameOverManager>().Setup(wonTheGame);

		ScreenTransitionManager.SetShowBlack(false, Vector2.zero);

		RemoveScreenFromLoading(Settings.SceneGameOver);
	}
	public void TransToTitle(string sceneFrom)
	{
		StartCoroutine(TransToTitleCo(sceneFrom));
	}

	IEnumerator TransToTitleCo(string sceneFrom)
	{
		if (IsScreenLoading(Settings.SceneTitle))
		{
			MMLogger.LogWarning($"TransToTitleCo called but screen already loading so stopping call");
			yield break;
		}

		AddScreenToLoading(Settings.SceneTitle);

		yield return ScreenTransitionManager.WaitForSetBlack(true, Vector2.zero);
		if (!sceneFrom.Equals(""))
		{
			yield return MainManager.Instance.SubtractSceneCo(sceneFrom);
		}
		OverworldMemory.ClearAll();
		MainManager.Instance.GoToTitle();
		ScreenTransitionManager.SetShowBlack(false, Vector2.zero);

		RemoveScreenFromLoading(Settings.SceneTitle);
	}
	
	public void TransToRewardReveal(string sceneFrom)
	{
		StartCoroutine(TransToRewardRevealCo(sceneFrom));
	}

	IEnumerator TransToRewardRevealCo(string sceneFrom)
	{
		if (IsScreenLoading(Settings.RewardReveal))
		{
			MMLogger.LogWarning($"TransToRewardRevealCo called but screen already loading so stopping call");
			yield break;
		}

		AddScreenToLoading(Settings.RewardReveal);

		yield return ScreenTransitionManager.WaitForSetBlack(true, Vector2.zero);

		if (!sceneFrom.Equals(""))
		{
			yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(sceneFrom));
		}

		yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.RewardReveal));
		yield return null;
		ScreenTransitionManager.SetShowBlack(false, Vector2.zero);

		RemoveScreenFromLoading(Settings.SceneTitle);
	}

	public void TransToPicker(string sceneFrom)
	{
		StartCoroutine(TransToPickerCo(sceneFrom));
	}

	IEnumerator TransToPickerCo(string sceneFrom)
	{
		AddScreenToLoading(Settings.SceneBodyPartPicker);

		var transPos = Vector2.zero;
		var player = FindObjectOfType<Player>();
		if (player != null)
		{
			transPos = player.transform.position;
		}
		yield return ScreenTransitionManager.WaitForSetBlack(true, transPos);

		if (!sceneFrom.Equals(""))
		{
			yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(sceneFrom));
		}
		yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.SceneBodyPartPicker));
		yield return null;
		FindObjectOfType<PartPickerManager>().Setup();
		ScreenTransitionManager.SetShowBlack(false, Vector2.zero);

		RemoveScreenFromLoading(Settings.SceneBodyPartPicker);
	}


	void AddScreenToLoading(string screen)
	{
		LoadingScreens.Add(screen);
	}

	void RemoveScreenFromLoading(string screen)
	{
		LoadingScreens.Remove(screen);
	}

	public bool IsScreenLoading(string screen)
	{
		return LoadingScreens.Contains(screen);
	}
}
