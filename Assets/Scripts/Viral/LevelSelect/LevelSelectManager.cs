using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
            public Text totalDays;
            public GameObject prePanel;
        }

        [SerializeField]
        GameObject[] buttons;
        [SerializeField]
        Info dynamicInfo;

        private Dictionary<Level.Location, Level> locationDictionary; // for reference to level objects through location
        private Dictionary<GameObject, Level> buttonDictionary;

        private TextInfo TI = new CultureInfo("en-US", false).TextInfo;

        private Dictionary<Level, int> levelDictionary; //stores a reference to active level object and days left (0 = remove)

        private GameObject activeButton = null;

        private Level playedLevel;

        private LevelGraph levelGraph = new LevelGraph();

        private void OnEnable()
        {
            UpdateDaysCountText();
            CheckAndUpdateActiveButton();
        }

        public void Initial()
        {
            dynamicInfo.prePanel.SetActive(true);
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
                    level.priority = Level.Priority.MEDIUM;
                    level.daysLeft = 2;
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.CALVES, Level.Location.Side.RIGHT)];
                    level.priority = Level.Priority.MEDIUM;
                    level.daysLeft = 2;
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    break;
                case GameManager.Data.Difficulty.ERGENT:
                    // start in both tricepts

                    level = locationDictionary[new Level.Location(
                            Level.Location.Type.TRICEPTS, Level.Location.Side.LEFT)];
                    level.priority = Level.Priority.MEDIUM;
                    level.daysLeft = 2;
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.TRICEPTS, Level.Location.Side.RIGHT)];
                    level.priority = Level.Priority.MEDIUM;
                    level.daysLeft = 2;
                    levelDictionary.Add(level, 2);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.MEDIUM);

                    break;
                case GameManager.Data.Difficulty.TERMINAL:
                    // start in heart and brain

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.BRAIN, Level.Location.Side.NONE)];
                    level.priority = Level.Priority.HIGH;
                    level.daysLeft = 2;
                    levelDictionary.Add(level, 3);
                    FindButtonByLevel(level).
                        GetComponent<LevelSelectButton>().Activate(true, Level.Priority.HIGH);

                    level = locationDictionary[new Level.Location(
                        Level.Location.Type.HEART, Level.Location.Side.NONE)];
                    level.priority = Level.Priority.HIGH;
                    level.daysLeft = 2;
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
            dynamicInfo.priorityLevel.text = TI.ToTitleCase(level.priority.ToString().ToLower())+" Priority";
            dynamicInfo.daysLeft.text = 
                levelDictionary[level].ToString()+
                " Day" + (levelDictionary[level] == 1 ? "" : "s") +
                " Left";//level.daysLeft.ToString()+" Days Left";
        }

        public void UpdateDaysCountText()
        {
            dynamicInfo.totalDays.text =
                GameManager.Instance.data.daysAlive + "\nDay" +
                (GameManager.Instance.data.daysAlive == 1 ? "" : "s") +
                "\nAlive";
        } 

        public void CheckAndUpdateActiveButton()
        {
            if (activeButton != null)
            {
                if (!levelDictionary.ContainsKey(buttonDictionary[activeButton]))
                {
                    dynamicInfo.prePanel.SetActive(true);
                    activeButton = null;
                }
                else
                {
                    DisplayLevelSelectInfo(buttonDictionary[activeButton]);
                }
            }
        }

        public void SetNextDayLevels()
        {
            ProcessDailyLevels();
            AddDailyLevel();
        }

        public void OnLevelButton(GameObject button)
        {
            dynamicInfo.prePanel.SetActive(false);

            activeButton = button;
            DisplayLevelSelectInfo(buttonDictionary[button]);
        }

        public void OnSelectButton()
        {
            playedLevel = buttonDictionary[activeButton];
            // launch level
            //buttonDictionary[activeButton].priority;

            GameManager.Instance.ChangeState(GameManager.State.INGAME);
            Time.timeScale = 1;
            SceneManager.LoadScene(1);

            //Debug.Log("?");

        }

        private void ProcessDailyLevels()
        { // called at end of day
            List<Level> keys = new List<Level>(levelDictionary.Keys);
            List<Level> toRemove = new List<Level>(); // to prevent out of sync error
            //DebugList(keys);
            foreach (Level level in keys)
            {
                levelDictionary[level] -= 1;
                if (levelDictionary[level] <= 0)
                {
                    //Debug.Log(level.location.type.ToString() + " " + level.location.side.ToString());
                    toRemove.Add(level);
                    FindButtonByLevel(level).GetComponent<LevelSelectButton>().Activate(false);
                }
            }
            foreach (Level level in toRemove)
            {
                levelDictionary.Remove(level);
            }
        }

        private void AddDailyLevel()
        { // adds the necessary daily levels
            int index = 0;
            int totalLevels = 2;
            // algorithm for totalLevels here <=
            int severity = 2;

            List<Level> keys = new List<Level>(levelDictionary.Keys);
            //DebugList(keys);
            foreach (Level level in keys)
            {
                ++index;
                Level random = locationDictionary[
                    levelGraph.GetNextLocationRandomly(level.location)];
                //Debug.Log(level.location.type.ToString() + " " + level.location.side.ToString());
                //Debug.Log(random.location.type.ToString() + " " + random.location.side.ToString());
                random.priority = (Level.Priority)Enum.Parse(typeof(Level.Priority), severity.ToString());
                if (!levelDictionary.ContainsKey(random))
                {
                    levelDictionary.Add(random, severity);
                    FindButtonByLevel(locationDictionary[random.location]).
                        GetComponent<LevelSelectButton>().
                            Activate(
                                true, 
                                (random.location.type >= Level.Location.Type.BRAIN && 
                                random.location.type <= Level.Location.Type.HEART) 
                                    ? Level.Priority.HIGH : 
                                        (random.location.type >= Level.Location.Type.COLON && 
                                        random.location.type <= Level.Location.Type.ABDOMEN) 
                                            ? Level.Priority.MEDIUM : Level.Priority.LOW); // sorry
                }
                else
                {
                    levelDictionary[level] = severity;
                }

                if (index >= totalLevels)
                    break;
            }
        }

        private GameObject FindButtonByLevel(Level level)
        {
            if (buttonDictionary == null)
                throw new ArgumentNullException("buttonDictionary");

            foreach (KeyValuePair<GameObject, Level> pair in buttonDictionary)
                if (level.Equals(pair.Value)) return pair.Key;

            throw new Exception("the value is not found in the dictionary");
        }

        private void DebugList<T>(List<T> list)
        {
            string sum = "";
            foreach (T t in list)
                sum += t + " ";
            Debug.Log(sum);
        }
    }
}
