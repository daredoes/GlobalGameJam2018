using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

namespace Viral.ControlSystem
{
    public struct Input
    {
        public Vector3 MoveInput;
        public bool JumpInput;
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

    public class InputController : MonoBehaviour
    {
        public Input Current;
        [SerializeField]
        protected ControllerStateMachine csm;

        public ControllerStateMachine CSM
        {
            get
            {
                if (csm == null)
                {
                    csm = GetComponent<ControllerStateMachine>();
                }
                return csm;
            }
        }


        float Horizontal
        {
            get
            {
               return CrossPlatformInputManager.GetAxis("Horizontal");
            }
        }

        float Vertical
        {
            get
            {
               return CrossPlatformInputManager.GetAxis("Vertical");
            }
        }

        // Use this for initialization
        void Start()
        {
            Current = new Input();
        }

        // Update is called once per frame
        void Update()
        {
            if (CSM.IsActive)
            {
                Vector3 moveInput = new Vector3(Horizontal, 0, Vertical);
                bool jumpInput = CrossPlatformInputManager.GetButtonDown("Jump");

                Current = new Input()
                {
                    MoveInput = moveInput,
                    JumpInput = jumpInput
                };
            }
            else
            {
                Current = new Input()
                {
                    MoveInput = Vector3.zero,
                    JumpInput = false,
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
