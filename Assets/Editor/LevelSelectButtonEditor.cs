using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Viral.LevelSelect
{
    [CustomEditor(typeof(LevelSelectButton))]
    public class LevelSelectButtonEditor : Editor {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LevelSelectButton b = (LevelSelectButton)target;
        }
    }
}
