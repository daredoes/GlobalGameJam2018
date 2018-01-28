using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

namespace Viral
{
    [CustomEditor(typeof(GUIManager))]
    public class GUIManagerEditor : Editor
    {
        [Header("Editor")]
        bool fold = true;

        public int panelIndex = 0;

        private string[] panels;

        private void ReloadList()
        {
            GUIManager gui = GUIManager.Instance;

            //Debug.Log(gui);
            gui.PopulatePanelListComplete();
            panels = null;
            panels = gui.panelList.Select(I => I.name).ToArray();
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            fold = EditorGUILayout.InspectorTitlebar(fold, GUIManager.Instance);

            if (fold)
            {
                EditorGUILayout.Space();

                ReloadList();
                int oldPanelIndex = panelIndex;
                panelIndex = EditorGUILayout.Popup("Panel", panelIndex, panels, EditorStyles.popup);
                if (panelIndex != oldPanelIndex)
                    GUIManager.Instance.TogglePanel(GUIManager.Instance.panelList[panelIndex]);

                GUIManager script = (GUIManager)target;
                if (GUILayout.Button("TogglePanel"))
                {
                    GUIManager.Instance.TogglePanel(GUIManager.Instance.panelList[panelIndex]);
                }
            }
        }
    }
#endif
}
