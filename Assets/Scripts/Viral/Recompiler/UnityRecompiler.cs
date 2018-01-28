using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class UnityRecompiler : MonoBehaviour {

    public void Refresh()
    {
        AssetDatabase.Refresh();
    }

    public void RecompileAllAssets()
    { // https://forum.unity.com/threads/force-script-recompilation.176572/
        AssetDatabase.StartAssetEditing();
        string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string assetPath in allAssetPaths)
        {
            MonoScript script = AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript)) as MonoScript;
            if (script != null)
            {
                AssetDatabase.ImportAsset(assetPath);
            }
        }
        AssetDatabase.StopAssetEditing();
    }
}
#endif
