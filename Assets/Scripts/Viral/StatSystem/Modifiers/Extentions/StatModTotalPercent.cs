using UnityEngine;
using System.Collections;
namespace Viral.StatSystem
{
    //example mod
    //this takes the total statValue and takes the modValue percentage to add back to the stat value
    public class StatModTotalPercent : StatModifier
    {
        #region Constructors
        public StatModTotalPercent() : base() { }

        public StatModTotalPercent(string id, float value) : base(id, value) { }

        public StatModTotalPercent(string id, float value, bool stacks) : base(id, value, stacks) { }
        #endregion
        
        //this mod order
        public override int Order
        {
            get
            {
                return 3;
            }
        }

        //this takes the total statValue and takes the modValue percentage to add back to the stat value
        public override int ApplyModifier(int statValue, float modValue)
        {
            return (int)(statValue * modValue);
        }
    }
}