
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BattleController))]
public class BattleControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
		DrawDefaultInspector();

		if (BattleController.Instance == null)
		{
			EditorGUILayout.LabelField($"Game Not running");
			return;
		}

		EditorGUILayout.LabelField($"battle state: {BattleController.Instance.BattleState}");

		if (BattleController.Instance.Player != null &&
			BattleController.Instance.Enemy != null)
		{
			EditorGUILayout.LabelField($"player health: {BattleController.Instance.Player.Body}");
			EditorGUILayout.LabelField($"enemy health: {BattleController.Instance.Enemy.Body}");
		}
		else
		{
			EditorGUILayout.LabelField($"not in battle");
		}

    }
}
#endif