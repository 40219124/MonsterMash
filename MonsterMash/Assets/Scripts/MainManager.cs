using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance { get; private set; }
    [SerializeField]
    string OpeningScene;
    [SerializeField]
    float LoadDelay = 2.0f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            enabled = false;
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadOverworld());
    }

    // ~~~ debug
    IEnumerator LoadOverworld()
    {
        yield return new WaitForSeconds(LoadDelay);
        SceneManager.LoadSceneAsync(OpeningScene, LoadSceneMode.Additive);
    }

    public void AddScene(string scene)
    {
        StartCoroutine(AddSceneCo(scene));
    }
    public IEnumerator AddSceneCo(string scene)
    {
        if (!SceneManager.GetSceneByName(scene).isLoaded)
        {
            yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        }
    }

    public void SubtractScene(string scene)
    {
        StartCoroutine(SubtractSceneCo(scene));
    }
    public IEnumerator SubtractSceneCo(string scene)
    {
        if (SceneManager.GetSceneByName(scene).isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(scene);
        }
    }
}
