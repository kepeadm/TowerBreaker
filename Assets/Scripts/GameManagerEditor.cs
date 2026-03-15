#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GameManager gm = (GameManager)target;

        EditorGUILayout.Space(10);
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("PlayerData 리셋", GUILayout.Height(40)))
        {
            if (EditorUtility.DisplayDialog(
                "PlayerData 리셋",
                "저장 데이터를 초기화할까요?",
                "리셋", "취소"))
            {
                gm.ResetPlayerData();
            }
        }
        GUI.backgroundColor = Color.white;
    }
}
#endif