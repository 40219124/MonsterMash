using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
	[SerializeField] Camera LocalCamera;

	public static MainManager Instance { get; private set; }

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			enabled = false;
			return;
		}
		AddScene(Settings.TransitionOverlay);
		AdditiveSceneManager.AudioListener = LocalCamera;
	}

	void Start()
	{
		StartCoroutine(GoToTitleCo());
	}

	void Update()
	{

		if (Input.GetButton("Exit"))
		{
			Debug.Log("Quitting");
			Application.Quit();
		}
	}

	public void AddScene(string scene)
	{
		StartCoroutine(AddSceneCo(scene));
	}
	public IEnumerator AddSceneCo(string scene)
	{
		MMLogger.Log($"AddSceneCo called with scene: {scene}");

		if (!SceneManager.GetSceneByName(scene).isLoaded)
		{
			yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
		}
		else
		{
			MMLogger.LogError($"AddSceneCo called with scene: {scene} that is already loaded");
		}
	}

	public void SubtractScene(string scene)
	{
		StartCoroutine(SubtractSceneCo(scene));
	}
	public IEnumerator SubtractSceneCo(string scene)
	{
		MMLogger.Log($"SubtractSceneCo called with scene: {scene}");

		if (SceneManager.GetSceneByName(scene).isLoaded)
		{
			yield return SceneManager.UnloadSceneAsync(scene);
		}
		else
		{
			MMLogger.LogError($"SubtractSceneCo called with scene: {scene} that is not loaded");
		}
	}

	public void GoToTitle()
	{
		StartCoroutine(GoToTitleCo());
	}
	public IEnumerator GoToTitleCo()
	{
		if (SceneManager.GetSceneByName(Settings.SceneOverworldMemory).isLoaded)
		{
			yield return StartCoroutine(SubtractSceneCo(Settings.SceneOverworldMemory));
		}
		yield return StartCoroutine(AddSceneCo(Settings.SceneTitle));
	}

	public void StartGame()
	{
		StartCoroutine(StartGameCo());
	}
	public IEnumerator StartGameCo()
	{
		yield return StartCoroutine(SubtractSceneCo(Settings.SceneTitle));
		yield return StartCoroutine(AddSceneCo(Settings.SceneOverworldMemory));
	}
}
