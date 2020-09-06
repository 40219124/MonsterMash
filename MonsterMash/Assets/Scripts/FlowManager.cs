using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OverworldMemory))]
public class FlowManager : MonoBehaviour
{
    public enum ERoomApproach { none = -1, FirstTime, FromBattle };
    private ERoomApproach approach = ERoomApproach.none;
    private void Start()
    {
        approach = ERoomApproach.FirstTime;
        TransToOverworld();
    }
    public void TransOverworldToBattle()
    {
        StartCoroutine(TransOverworldToBattleCo());
    }

    private IEnumerator TransOverworldToBattleCo()
    {
        yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(Settings.SceneOverworld));
        yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.SceneBattle));
        yield return null;
        BattleController bc = FindObjectOfType<BattleController>();
        if (bc != null)
        {
            bc.SetupBattle(OverworldMemory.GetCombatProfile(true), OverworldMemory.GetCombatProfile(false));
        }
    }

    // transition to overworld from unknown scene
    public void TransToOverworld(string sceneFrom = "")
    {
        StartCoroutine(TransToOverworldCo(sceneFrom));
    }

    private IEnumerator TransToOverworldCo(string sceneFrom)
    {
		if (!sceneFrom.Equals(""))
		{
			approach = ERoomApproach.FromBattle;
			yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(sceneFrom));
		}

		var loadEnemyFromMemory = approach != ERoomApproach.FirstTime;

		yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.SceneOverworld));
		yield return null;
		FindObjectOfType<EnemySpawner>().SpawnEnemies(loadEnemyFromMemory); // ~~~ spawn based on ERoomApproach
		if (OverworldMemory.GetEnemyProfiles().Count == 0)
		{
			FindObjectOfType<CurrentRoom>().PlaceDoors();
		}
		if (approach == ERoomApproach.FromBattle)
		{
			Player p = FindObjectOfType<Player>();
			p.transform.position = OverworldMemory.GetPlayerPosition();
			p.Profile = OverworldMemory.GetCombatProfile(true);
		}
    }

    public void TransToGameOver(string sceneFrom, bool wonTheGame)
    {
        StartCoroutine(TransToGameOverCo(sceneFrom, wonTheGame));
    }

    private IEnumerator TransToGameOverCo(string sceneFrom, bool wonTheGame)
    {
        if (!sceneFrom.Equals(""))
        {
            yield return MainManager.Instance.SubtractSceneCo(sceneFrom);
        }
        yield return MainManager.Instance.AddSceneCo(Settings.SceneGameOver);
		yield return null;
        FindObjectOfType<GameOverManager>().Setup(wonTheGame);
    }
    public void TransToTitle(string sceneFrom)
    {
        StartCoroutine(TransToTitleCo(sceneFrom));
    }

    private IEnumerator TransToTitleCo(string sceneFrom)
    {
        if (!sceneFrom.Equals(""))
        {
            yield return MainManager.Instance.SubtractSceneCo(sceneFrom);
        }
        OverworldMemory.ClearAll();
        MainManager.Instance.GoToTitle();
    }

    public void TransToPicker(string sceneFrom)
    {
        StartCoroutine(TransToPickerCo(sceneFrom));
    }

    private IEnumerator TransToPickerCo(string sceneFrom)
    {
        if (!sceneFrom.Equals(""))
		{
            yield return StartCoroutine(MainManager.Instance.SubtractSceneCo(sceneFrom));
        }
        yield return StartCoroutine(MainManager.Instance.AddSceneCo(Settings.SceneBodyPartPicker));
        yield return null;
        FindObjectOfType<PartPickerManager>().Setup();
    }

	public void TransToNextRoom(string sceneFrom, Vector2Int roomDirection)
    {
        StartCoroutine(TransToNextRoomCo(sceneFrom, roomDirection));
    }

    private IEnumerator TransToNextRoomCo(string sceneFrom, Vector2Int roomDirection)
    {
		ProceduralDungeon.Instance.MoveRoom(roomDirection);

        yield return StartCoroutine(TransToOverworldCo(sceneFrom));
    }
}
