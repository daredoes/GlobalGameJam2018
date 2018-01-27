using UnityEngine;
using System;
using System.Collections;
namespace Viral.StatSystem
{
    //Inherit from this abstract class to create different stat modifiers for different stat effects
    //four different examples in the extentions folder
    public abstract class StatModifier
    {
        private string _id;
        //the mod value
        private float _value = 0f;

        public event EventHandler OnValueChange;

        #region Constructors
        public StatModifier()
        {
            //empty constructor
            Value = 0;
            Stacks = true;
        }

        public StatModifier(string id, float value)
        {
            ID = id;
            Value = value;
            Stacks = true;
        }

        public StatModifier(string id, float value, bool stacks)
        {
            ID = id;
            Value = value;
            Stacks = stacks;
        }
        #endregion

        #region Properties - Getters/Setters
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        //the order in which it lands in the stack of modifiers
        public abstract int Order { get; }

        //whether this mod can be stacked or not
        public bool Stacks { get; set; }

        public float Value
        {
            get { return _value; }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (OnValueChange != null)
                    {
                        OnValueChange(this, null);
                    }
                }
            }
        }
        #endregion

        //returns the value of the corresponding mod that effects it
        public abstract int ApplyModifier(int statValue, float modValue);
    }
}