using UnityEngine;
using System;
using Viral.StatSystem.Interfaces;

namespace Viral.StatSystem
{
    /*
    Third layer of stat inheritence.

    Added Functionality:
        Current and Max Stat Value
    
    Stat Vital is a complete Stat
        Ex: Health
            - has current stat value: 45 HP
            - has max stat value: 100 HP
    */
    public class StatVital : StatAttribute, IStatCurrentValueChanged
    {
        private int _currentValue; //the stats current value

        public event EventHandler OnCurrentValueChanged; //event for when the stat value changes  

        #region Constructors 
        public StatVital()
        {
            _currentValue = 0;
        }
        #endregion

        #region Properties - Getters/Setters
        new public int Value
        {
            get
            {
                if (_currentValue > Max)
                {
                    _currentValue = Max;
                }
                else if (_currentValue < 0)
                {
                    _currentValue = 0;
                }
                return _currentValue;
            }
            set
            {
                if (_currentValue != value)
                {
                    _currentValue = value;
                    TriggerCurrentValueChange();
                }
            }
        }

        public int Max
        {
            get { return base.Value; }
        }
        #endregion

        public void RestoreCurrentValueToMax()
        {
            Value = Max;
        }

        private void TriggerCurrentValueChange()
        {
            if (OnCurrentValueChanged != null)
            {
                OnCurrentValueChanged(this, null);
            }
        }
    }
}