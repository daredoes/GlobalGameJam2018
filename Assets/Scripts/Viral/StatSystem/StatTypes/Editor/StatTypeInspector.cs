using UnityEngine;
using UnityEditor;
using Viral.StatSystem.Database;

namespace Viral.StatSystem.Editor
{
    [CustomEditor(typeof(StatTypeDatabase))]
    public class StatTypeInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GUILayout.Label("Database that stores all StatTypes");

            if (GUILayout.Button("Open Editor Window"))
            {
                StatTypeEditor.ShowWindow();
            }
        }
    }
}