using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Viral.LevelSelect
{
    [Serializable]
    public class LevelSelectButton : Button {

        public LevelSelectManager levelSelectManager;
        public Level.Location location = new Level.Location();

        public GameObject alertObject;

        private Image alertImage;

        private bool isActive = false;

        public void OnButton()
        {
            levelSelectManager.OnLevelButton(gameObject);
        }

        public void Activate(bool onOff, Level.Priority priority = Level.Priority.NULL)
        {
            isActive = onOff;
            alertObject.SetActive(onOff);
            alertImage = alertObject.GetComponent<Image>();

            switch (priority)
            {
                case Level.Priority.NULL:
                    alertObject.SetActive(false);
                    break;
                case Level.Priority.LOW:
                    alertImage.color = Color.green;
                    break;
                case Level.Priority.MEDIUM:
                    alertImage.color = Color.yellow;
                    break;
                case Level.Priority.HIGH:
                    alertImage.color = Color.red;
                    break;
            }
        }

        private void Update()
        {
            if (isActive)
            {
                // animation
            }
        }
    }
}
