using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

namespace Viral.LevelSelect
{
    public class LevelSelectManager : MonoBehaviour
    {

        public struct LevelButton
        {
            public Level level;
            public Level.Location location;
            public bool isActive;
            public int daysLeft;
        }

        [Serializable]
        public struct Info
        {
            public Text levelName;
            public Text priorityLevel;
            public Text daysLeft;
        }

        [SerializeField]
        GameObject[] buttons;
        [SerializeField]
        Info dynamicInfo;

        private Dictionary<Level.Location, Level> locationDictionary; // for reference to level objects through location
        private Dictionary<GameObject, Level> buttonDictionary;

        private TextInfo TI = new CultureInfo("en-US", false).TextInfo;

        private Dictionary<Level, int> levelDictionary; //stores a reference to active level object and days left (0 = remove)

        private GameObject activeButton;

        private Level playedLevel;

        private void OnEnable()
        {
            //InitializeLevelDictionary();
            PopulateButtonDictionary();
            InitializeLevelDictionary();
        }

        private void InitializeLevelDictionary()
        {
            levelDictionary = new Dictionary<Level, int>();
            Level level;

            switch (GameManager.Instance.data.difficulty) {
                case GameManager.Data.Difficulty.NULL:
                    Debug.Log("ERROR: Difficulty set to NULL.");
                    break;
                case GameManager.Data.Difficulty.BENIGN:
                    // start in both calves

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.CALVES, Level.Location.Side.LEFT)];
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.CALVES, Level.Location.Side.RIGHT)];
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    break;
                case GameManager.Data.Difficulty.ERGENT:
                    // start in both tricepts

                    level = locationDictionary[new Level.Location(
                            Level.Location.Type.TRICEPTS, Level.Location.Side.LEFT)];
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.TRICEPTS, Level.Location.Side.RIGHT)];
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    break;
                case GameManager.Data.Difficulty.TERMINAL:
                    // start in heart and brain

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.BRAIN, Level.Location.Side.NONE)];
                    levelDictionary.Add(level, 3);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.HIGH);

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.HEART, Level.Location.Side.NONE)];
                    levelDictionary.Add(level, 3);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.HIGH);

                    break;
            }
        }

        private void PopulateButtonDictionary()
        {
            buttonDictionary = new Dictionary<GameObject, Level>();
            locationDictionary = new Dictionary<Level.Location, Level>();
            for (int i = 0; i < buttons.Length; ++i) {
                LevelSelectButton lsb = buttons[i].GetComponent<LevelSelectButton>();
                lsb.Activate(false);
                Level l = new Level(lsb.location);
                buttonDictionary.Add(buttons[i], l);
                locationDictionary.Add(lsb.location, l);
            }
        }

        private void DisplayLevelSelectInfo(Level level)
        {
            dynamicInfo.levelName.text = TI.ToTitleCase(level.name);
            dynamicInfo.priorityLevel.text = TI.ToTitleCase(level.priority.ToString());
            dynamicInfo.daysLeft.text = level.daysLeft.ToString();
        }

        public void OnLevelButton(GameObject button)
        {
            activeButton = button;
            DisplayLevelSelectInfo(buttonDictionary[button]);
        }

        public void OnSelectButton()
        {
            playedLevel = buttonDictionary[activeButton];
            // launch game
        }

        private GameObject FindButtonByLevel(Level level)
        {
            if (buttonDictionary == null)
                throw new ArgumentNullException("buttonDictionary");

            foreach (KeyValuePair<GameObject, Level> pair in buttonDictionary)
                if (level.Equals(pair.Value)) return pair.Key;

            throw new Exception("the value is not found in the dictionary");
        }
    }
}
