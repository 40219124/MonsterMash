using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(OverworldMemory))]
public class FlowManager : MonoBehaviour
{
    private void Start()
    {
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
            bc.SetupBattle(OverworldMemory.GetProfile(true), OverworldMemory.GetProfile(false));
        }
    }
}
