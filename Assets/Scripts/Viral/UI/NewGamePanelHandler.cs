using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.UI
{
    public class NewGamePanelHandler : MonoBehaviour
    {

        public void BenignButton()
        {
            Progress(GameManager.Data.Difficulty.BENIGN);
        }

        public void ErgentButton()
        {
            Progress(GameManager.Data.Difficulty.ERGENT);
        }

        public void TerminalButton()
        {
            Progress(GameManager.Data.Difficulty.TERMINAL);
        }

        private void Progress(GameManager.Data.Difficulty difficulty)
        {
            GameManager.Instance.data.difficulty = difficulty;
            GUIManager.Instance.StartLevelSelect();

            GameManager.Instance.ChangeState(GameManager.State.NEWGAME);
        }
    }
}
