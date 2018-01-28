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

        static protected GameManager _instance;
        static public GameManager Instance { get { return _instance; } }

        public Data data;

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
        }
    }
}
