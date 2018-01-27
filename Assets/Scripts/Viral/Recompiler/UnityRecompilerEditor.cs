using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(UnityRecompiler))]
public class UnityRecompilerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UnityRecompiler myScript = (UnityRecompiler)target;
        if (GUILayout.Button("Refresh"))
        {
            myScript.Refresh();
        }
        if (GUILayout.Button("Recompile All Assets"))
        {
            myScript.RecompileAllAssets();
        }
    }
}
#endif
