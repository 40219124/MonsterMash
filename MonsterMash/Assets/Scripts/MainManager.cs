using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [SerializeField]
    string OpeningScene;
    [SerializeField]
    float LoadDelay = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadOverworld());
    }

    IEnumerator LoadOverworld()
    {
        yield return new WaitForSeconds(LoadDelay);
        SceneManager.LoadSceneAsync(OpeningScene, LoadSceneMode.Additive);
    }

    public IEnumerator AddScene(string scene)
    {
        if (!SceneManager.GetSceneByName(scene).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        }
    }

    public IEnumerator SubtractScene(string scene)
    {
        if (SceneManager.GetSceneByName(scene).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(scene);
        }
    }

    public void TransOverworldToBattle()
    {
        StartCoroutine(TransOverworldToBattleCo());
    }

    private IEnumerator TransOverworldToBattleCo()
    {
        yield return StartCoroutine(SubtractScene("Tim"));
        yield return StartCoroutine(AddScene("Louie"));
    }
}
