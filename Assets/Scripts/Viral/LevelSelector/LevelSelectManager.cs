using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Viral
{
    public class LevelSelectManager : MonoBehaviour
    {

        public struct LevelButton
        {
            public Level level;
            public Level.Location location;
        }

        [SerializeField]
        GameObject[] buttons;

        private Dictionary<GameObject, LevelButton> buttonDictionary;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void PopulateDictionary()
        {
            buttonDictionary = new Dictionary<GameObject, LevelButton>();
            for (int i = 0; i < buttons.Length; ++i) {
                LevelButton lb = new LevelButton();
                lb.location = buttons[i].GetComponent<LevelSelectButton>().location;
                buttonDictionary.Add(buttons[i], lb);
            }
        }

        void DisplayLevelButtonInfo(LevelButton levelButton)
        {

        }
    }
}
