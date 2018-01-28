using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.UI
{
    public class StartMenuPanelHandler : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField]
        GameObject NewGamePanel;
        [SerializeField]
        GameObject LoadGamePanel;
        [SerializeField]
        GameObject LeaderboardPanel;

        void Start()
        {
            NewGamePanel.SetActive(false);
            LoadGamePanel.SetActive(false);
            LeaderboardPanel.SetActive(false);
        }

        public void NewGameButton()
        {
            NewGamePanel.SetActive(true);
        }

        public void LoadGameButton()
        {
            LoadGamePanel.SetActive(true);
        }

        public void LeaderboardButton()
        {
            LeaderboardPanel.SetActive(true);
        }
    }
}
