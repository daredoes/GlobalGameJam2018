using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

namespace Viral.ControlSystem
{
    public struct NoInput
    {
        public Vector3 MoveInput;
        public Vector3 JumpInput;
        public Vector3 DoubleJumpInput;
        public Vector3 DashInput;
        public bool FlipInput;
        public bool BlockInput;
        public bool AttackInput;
        public bool ThrowInput;
        public bool MagicInput;
        public bool SwitchPrimaryInput;
        public bool SwitchSecondaryInput;
        public bool SwitchThrowableInput;
        public bool SwitchProjectileInput;
        public bool OpenInventory;
        public bool ItemPickUp;
    }

    public class NoInputController : MonoBehaviour
    {
        public NoInput Current;
        [SerializeField]
        protected AiMachine am;

        [SerializeField]
        private AiMovement brain;
        private Vector3 _movement;

        public AiMachine AM
        {
            get
            {
                if (am == null)
                {
                    am = GetComponent<AiMachine>();
                }
                return am;
            }
        }


        public float Horizontal
        {
            get
            {
                return _movement.x;
            }
            set{
                _movement = new Vector3(value, _movement.y, _movement.z);
            }
        }

        public float Vertical
        {
            get
            {
                return _movement.z;
            }
            set
            {
                _movement = new Vector3(_movement.x, _movement.y, value);
            }
        }


        // Use this for initialization
        void Start()
        {
            Current = new NoInput();
            _movement = new Vector3(0, 0, 0);
        }

        // Update is called once per frame
        void Update()
        {
            /*
             * Here it should be determined what exactly the AI is doing
             * This is going to be handled by calling the update function of a script
             */

            if(brain){
                Current = brain.Act(AM);
            }
           
            else
            {
                Current = new NoInput()
                {
                    MoveInput = Vector3.zero,
                    JumpInput = Vector3.zero,
                    DoubleJumpInput = Vector3.zero,
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
}
