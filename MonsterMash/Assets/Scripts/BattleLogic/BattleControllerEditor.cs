using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(BattleController))]
public class BattleControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {

		if (BattleController.Instance?.Player != null &&
			BattleController.Instance.Enemy != null)
		{
			EditorGUILayout.LabelField($"player health: {BattleController.Instance.Player.Body}");
			EditorGUILayout.LabelField($"enemy health: {BattleController.Instance.Enemy.Body}");
		}
		else
		{
			EditorGUILayout.LabelField($"not setup yet");
		}

    }
}