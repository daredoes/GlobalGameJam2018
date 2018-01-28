using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Viral
{
    public class GUIManager : MonoBehaviour
    {

        static protected GUIManager _instance;
        static public GUIManager Instance
        { // https://gist.github.com/simonbroggi/720ea3388ae10ead6771
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                GameObject o;
                if (Application.isPlaying)
                {
                    o = (GameObject)Instantiate(Resources.Load("GUIManager"));
                }
                else
                {
                    o = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/GUIManager.prefab");
                    if (o == null)
                    {
                        GameObject tempGO = new GameObject("GUIManager", typeof(GUIManager));
                        o = UnityEditor.PrefabUtility.CreatePrefab("Assets/Resources/GUIManager.prefab", tempGO);
                        DestroyImmediate(tempGO);
                    }
                }
#else
                    GameObject o = (GameObject)Instantiate(Resources.Load("ResourcesSingleton"));
#endif
                    _instance = ((GameObject)o).GetComponent<GUIManager>();
                }
                return _instance;
            }
        }

        [Header("Panels")]
        [SerializeField]
        GameObject StartMenuPanel;
        [SerializeField]
        GameObject SettingsPanel;
        [SerializeField]
        GameObject InGamePanel;
        [SerializeField]
        GameObject LevelSelectPanel;
        [SerializeField]
        GameObject GameOverPanel;

        [Header("References")]
        public LevelSelect.LevelSelectManager LSManager;

        [HideInInspector]
        public List<GameObject> panelList;

        private bool isSettingsOpen = false;

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.Log(this.GetType().Name + " is already in play. Deleting new!", gameObject);
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Start()
        {
            PopulatePanelListComplete();
        }

        /// <summary>
        /// Method called to set a specified panel as active while setting all others, besides exempts, as deactive.
        /// List of panel references can be found at GUIManager.panelList
        /// </summary>
        /// <param name="panel">A reference to a panel, to be set as active, from panelList</param>
        /// <param name="exempt">A list of references to panels, to be exempt from being set as deactive, from panelList</param>
        public void TogglePanel(GameObject panel, GameObject[] exempt = null)
        {
            if (exempt == null)
            {
                exempt = new GameObject[] { };
            }
            panel.SetActive(true);
            foreach(GameObject go in panelList)
            {
                if (go != panel && !exempt.Contains(go))
                {
                    go.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Method called to populate panelList with a hard coded GameObject array. 
        /// Any togglable panel should be added to the GameObject array.
        /// </summary>
        public void PopulatePanelListComplete()
        {
            PopulatePanelList(new GameObject[]
            {
                StartMenuPanel   == null ? GameObject.Find("StartMenuPanel")   : StartMenuPanel,
                InGamePanel      == null ? GameObject.Find("InGamePanel")      : InGamePanel,
                SettingsPanel    == null ? GameObject.Find("SettingsPanel")    : SettingsPanel,
                LevelSelectPanel == null ? GameObject.Find("LevelSelectPanel") : LevelSelectPanel,
                GameOverPanel    == null ? GameObject.Find("GameOverPanel")    : GameOverPanel
            });
        }

        private void PopulatePanelList(GameObject[] goArr)
        {
            panelList = new List<GameObject>();
            for (int i = 0; i < goArr.Length; ++i)
            {
                panelList.Add(goArr[i]);
            }
        }

        public void SettingsButton()
        {
            TogglePanel(SettingsPanel, new GameObject[] { StartMenuPanel });
        }

        public void StartLevelSelect() // toggles level select
        {
            TogglePanel(LevelSelectPanel);
        }

        public void StartInGame()
        {
            TogglePanel(InGamePanel);
        }

        public void StartGameOver()
        {
            TogglePanel(GameOverPanel);
        }
    }
}
