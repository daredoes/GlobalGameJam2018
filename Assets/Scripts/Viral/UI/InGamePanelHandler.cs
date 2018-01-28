using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.UI
{
    public class InGamePanelHandler : MonoBehaviour
    {

        public void OnFinishLevelButton()
        {
            GameManager.Instance.ChangeState(GameManager.State.POSTGAME);
        }
    }
}
