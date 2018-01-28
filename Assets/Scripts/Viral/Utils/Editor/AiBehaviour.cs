using UnityEngine;
using System.Collections;
using UnityEditor;
using Viral.ControlSystem;

public class AiBehaviour : MonoBehaviour {
    [MenuItem("Assets/Create/AiBehaviour")]
    public static void CreateMyAsset()
    {
        AiMovement asset = ScriptableObject.CreateInstance<HeavyAi>();

        AssetDatabase.CreateAsset(asset, "Assets/HeavyAi.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
