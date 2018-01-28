using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.UI
{
    public class GameOverPanelHandler : MonoBehaviour
    {

        public void BackToLevelSelectButton()
        {
            GameManager.Instance.ChangeState(GameManager.State.PREGAME);
        }
    }
}
