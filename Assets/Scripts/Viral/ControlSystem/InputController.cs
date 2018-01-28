using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

namespace Viral.ControlSystem
{
    public struct Input
    {
        public Vector3 MoveInput;
        public Vector3 JumpInput;
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

    public class InputController : MonoBehaviour
    {
        public Input Current;
        private Vector3 jumpStats;
        private Vector3 dashStats;
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

        Vector3 Jump{
            get{
                return jumpStats;
            }
            set{
                jumpStats = value;
            }
        }

        Vector3 Dash{
            get{
                return dashStats;
            }
            set{
                dashStats = value;
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
                if(CrossPlatformInputManager.GetButtonDown("Jump")){
                    Jump = new Vector3(0, 4f, 20f);
                }
                else{
                    Jump = Vector3.zero;
                }
                if(CrossPlatformInputManager.GetButtonDown("Dash")){
                    Dash = new Vector3(10f, 1f, 1f);
                }
                else{
                    Dash = Vector3.zero;
                }


                Current = new Input()
                {
                    MoveInput = moveInput,
                    JumpInput = Jump,
                    DashInput = Dash
                };
            }
            else
            {
                Current = new Input()
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
}
