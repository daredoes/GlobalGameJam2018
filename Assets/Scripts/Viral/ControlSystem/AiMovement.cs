using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.ControlSystem {
    public abstract class AiMovement : ScriptableObject
    {
       
        public virtual NoInput Act(AiMachine am)
        {
            return new NoInput()
            {
                MoveInput = Vector3.zero,
                JumpInput = Vector3.zero,
                DashInput = Vector3.zero,
                FlipInput = false,
                BlockInput = false,
                AttackInput = false,
                MagicInput = false,
                ThrowInput = false,
                SwitchPrimaryInput = false,
                SwitchSecondaryInput = false,
                SwitchThrowableInput = false,
                SwitchProjectileInput = false,
                OpenInventory = false,
                ItemPickUp = false
            };
        }

    }   
}