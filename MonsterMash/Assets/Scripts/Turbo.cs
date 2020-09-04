#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TurboWindow : EditorWindow
{

	[MenuItem("MonsterMash/Turbo")]
	static void Init()
	{
		var window = (TurboWindow)GetWindow(typeof(TurboWindow));
		window.Show();

		window.titleContent = new GUIContent("Turbo");
		window.minSize = new Vector2(200, 32);
	}

	void OnGUI()
	{
		EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);

		Time.timeScale = EditorGUILayout.Slider("Time Scale", Time.timeScale, 0.0f, 10.0f);

		EditorGUILayout.BeginHorizontal();
		GUI.backgroundColor = Color.white;
		if (GUILayout.Button("0.1X Turbo"))
		{
			SetTime(0.1f);
		}
		GUI.backgroundColor = Color.red;
		if (GUILayout.Button("1X Turbo"))
		{
			SetTime(1f);
		}
		GUI.backgroundColor = Color.white;
		if (GUILayout.Button("2X Turbo"))
		{
			SetTime(2f);
		}
		if (GUILayout.Button("4X Turbo"))
		{
			SetTime(4f);
		}
		if (GUILayout.Button("10X Turbo"))
		{
			SetTime(10f);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUI.BeginChangeCheck();
		EditorGUI.EndDisabledGroup();
	}

	void SetTime(float time)
	{
		Time.timeScale = time;
	}
}
#endif