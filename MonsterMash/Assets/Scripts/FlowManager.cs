using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OverworldMemory))]
public class FlowManager : MonoBehaviour
{
	public static FlowManager Instance;

    private void Start()
    {
        TransToOverworld();
		Instance = this;
    }
    public void TransOverworldToBattle()
    {
        StartCoroutine(TransOverworldToBattleCo());
    }

    private IEnumerator TransOverworldToBattleCo()
    {
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
    }


    // transition to overworld from unknown scene
    public void TransToOverworld(string sceneFrom = "", bool isNewDungeon=false)
    {
        StartCoroutine(TransToOverworldCo(sceneFrom, isNewDungeon));
    }

    private IEnumerator TransToOverworldCo(string sceneFrom, bool isNewDungeon)
    {
		if (!sceneFrom.Equals(""))
		{
			yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(sceneFrom));
		}

		yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.SceneOverworld));
		FindObjectOfType<CurrentRoom>().Setup(sceneFrom == Settings.SceneOverworld && !isNewDungeon);

    }

    public void TransToGameOver(string sceneFrom, bool wonTheGame)
    {
        StartCoroutine(TransToGameOverCo(sceneFrom, wonTheGame));
    }

    private IEnumerator TransToGameOverCo(string sceneFrom, bool wonTheGame)
    {
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
    }
    public void TransToTitle(string sceneFrom)
    {
        StartCoroutine(TransToTitleCo(sceneFrom));
    }

    private IEnumerator TransToTitleCo(string sceneFrom)
    {
		yield return ScreenTransitionManager.WaitForSetBlack(true, Vector2.zero);
        if (!sceneFrom.Equals(""))
        {
            yield return MainManager.Instance.SubtractSceneCo(sceneFrom);
        }
        OverworldMemory.ClearAll();
        MainManager.Instance.GoToTitle();
		ScreenTransitionManager.SetShowBlack(false, Vector2.zero);
    }

    public void TransToPicker(string sceneFrom)
    {
        StartCoroutine(TransToPickerCo(sceneFrom));
    }

    private IEnumerator TransToPickerCo(string sceneFrom)
    {
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
    }
}
