using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral
{
    public class GameManager : MonoBehaviour
    {
        #region Data Struct
        public struct Data
        {
            public enum Difficulty
            {
                NULL = -1,
                BENIGN = 0,
                ERGENT = 1,
                TERMINAL = 2
            }

            public Difficulty difficulty;
            public int daysAlive;

            public Data(Difficulty? difficulty = null, int daysAlive = 0)
            {
                if (difficulty == null)
                    this.difficulty = new Difficulty();
                else
                    this.difficulty = (Difficulty)difficulty;

                this.daysAlive = daysAlive;
            }
        }
        #endregion

        public enum State
        {
            NULL = -1,
            NEWGAME = 0, // used as first pregame state
            PREGAME = 1, // anything before the level is selected
            INGAME = 2,  // once level is selected
            POSTGAME = 3 // after game over in level but before pregame again
        }

        static protected GameManager _instance;
        static public GameManager Instance { get { return _instance; } }

        public Data data;

        public State state;

        #region Unity Functions

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.LogWarning(GetType().Name + " is already in play. Deleting new!", gameObject);
                Destroy(gameObject);
            }
            else
            { _instance = this; }

            data = new Data();
            //ChangeState(State.NULL);
        }

        #endregion

        #region Game Functions

        public void Pause(bool onOff)
        {
            Time.timeScale = onOff ? 0 : 1;
        }

        #endregion

        #region State Functions

        public void ChangeState(State state)
        {
            Debug.Log("Changing GM state: " + state);
            this.state = state;
            switch (state)
            {
                case State.NULL:
                    Pause(true);
                    Debug.Log("Changing GameManager.state to NULL");
                    break;
                case State.NEWGAME:
                    OnNewGameStart();
                    break;
                case State.PREGAME:
                    OnPreGameStart();
                    break;
                case State.INGAME:
                    OnInGameStart();
                    break;
                case State.POSTGAME:
                    OnPostGameStart();
                    break;
            }
        }

        private void OnNewGameStart()
        {
            Pause(true);
            GUIManager.Instance.LSManager.Initial();
        }

        private void OnPreGameStart()
        {
            Pause(true);
            GUIManager.Instance.StartLevelSelect();
        }

        private void OnInGameStart()
        {
            Pause(false);
            GUIManager.Instance.StartInGame();
        }

        private void OnPostGameStart()
        {
            GUIManager.Instance.StartGameOver();
            // check for success ingame then
            GUIManager.Instance.LSManager.SetNextDayLevels();
        }

        #endregion
    }
}
