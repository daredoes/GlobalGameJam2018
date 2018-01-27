using UnityEngine;
using System.Collections;
namespace Viral.StatSystem
{
    //example mod
    //add Mod value to the base value of the stat
    public class StatModBaseAdd : StatModifier
    {
        #region Constructors
        public StatModBaseAdd() : base() { }

        public StatModBaseAdd(string id, float value) : base(id, value) { }

        public StatModBaseAdd(string id, float value, bool stacks) : base(id, value, stacks) { }
        #endregion

        //this mod order
        public override int Order
        {
            get
            {
                return 2;
            }
        }

        //this mod just returns the modValue which will later be added
        public override int ApplyModifier(int statValue, float modValue)
        {
            return (int)(modValue);
        }

    }
}