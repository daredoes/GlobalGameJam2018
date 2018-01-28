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
        public Vector3 SlamInput;
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
        private Vector3 jumpStats;
        [SerializeField]
        private Vector3 dashStats;
        [SerializeField]
        private Vector3 slamStats;
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
                if (CrossPlatformInputManager.GetButtonDown("Jump"))
                {
                    return jumpStats;
                }
                else
                {
                    return Vector3.zero;
                }
            }
            set{
                jumpStats = value;
            }
        }

        Vector3 Dash{
            get{
                if (CrossPlatformInputManager.GetButtonDown("Dash"))
                {
                    return dashStats;
                }
                else
                {
                    return Vector3.zero;
                }
            }
            set{
                dashStats = value;
            }
        }
        Vector3 Slam
        {
            get
            {
                if(CrossPlatformInputManager.GetButtonDown("Slam")){
                    return slamStats;
                }
                else {
                    return Vector3.zero;
                }

            }
            set
            {
                slamStats = value;
            }
        }

        bool Attack
        {
            get
            {
                if (CrossPlatformInputManager.GetButton("Attack"))
                {
                    return true;
                }
                return false;
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



                Current = new Input()
                {
                    MoveInput = moveInput,
                    JumpInput = Jump,
                    DashInput = Dash,
                    SlamInput = Slam,
                    AttackInput = Attack
                };
            }
            else
            {
                Current = new Input()
                {
                    MoveInput = Vector3.zero,
                    JumpInput = Vector3.zero,
                    DashInput = Vector3.zero,
                    SlamInput = Vector3.zero,
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
