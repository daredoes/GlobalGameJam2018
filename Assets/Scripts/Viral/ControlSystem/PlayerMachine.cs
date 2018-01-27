using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

namespace Viral.ControlSystem
{
    public class PlayerMachine : ControllerStateMachine
    {
        enum PlayerStates
        {
            Idle,
            Walk,
            Jump,
            Block,
            Juggle, // juggle / flinch state
            Attack_Melee,
            Attack_Magic,
            Throw,
            Fall
        }

        #region VARS
        int normalizedHorizontalSpeed;
        bool previouslyOnGround;
        float groundDamping = 20f;
        float inAirDamping = 5f;


        [SerializeField]
        private InputController _input;
        private CharacterController2D _controller;
        #endregion

        #region PROPERTIES
        public InputController Input
        {
            get
            {
                if (_input == null)
                {
                    _input = GetComponent<InputController>();
                }
                return _input;
            }
        }

        public CharacterController2D Controller
        {
            get
            {
                if(_controller == null)
                {
                    _controller = GetComponent<CharacterController2D>();
                }
                return _controller;
            }
        }

        public new bool MaintainingGround
        {
            get
            {
                return Controller.isGrounded;
            }
        }
      

        private Vector3 LocalMovement
        {
            get
            {
                Vector3 local = Vector3.zero;
                if (Input.Current.MoveInput.x != 0)
                {
                    local += transform.right * Input.Current.MoveInput.x;
                }
                if (Input.Current.MoveInput.z != 0)
                {
                    local += transform.forward * Input.Current.MoveInput.z;
                }
                return local;
            }
        }

        #endregion

        public void Start()
        {
            Initialize();
        }

        public override void Initialize()
        {
            //inventoryManager.OnItemEncounter += ItemEncountered;
            //inventoryManager.OnItemLeft += ItemLeft;
            currentState = PlayerStates.Idle;
            statCollection.Init();
            base.Initialize();
        }

        protected override void EarlyGlobalSuperUpdate()
        {
            // Put any code in here you want to run BEFORE the state's update function.
            // This is run regardless of what state you're in
            base.EarlyGlobalSuperUpdate();
            //Flip the character
            if ((Input.Current.MoveInput.x > 0 && !facingRight) || (Input.Current.MoveInput.x < 0 && facingRight)) { Flip(); }

          
        }

        protected override void LateGlobalSuperUpdate()
        {
            // Put any code in here you want to run AFTER the state's update function.
            // This is run regardless of what state you're in
            // Move the player by our velocity every frame
            base.LateGlobalSuperUpdate();
            //transform.position += moveDirection * Time.deltaTime;
            moveDirection.y += gravity * Time.deltaTime;
            Controller.move(moveDirection * Time.deltaTime);
            moveDirection = Controller.velocity;
            Debug.Log(moveDirection);
        }

        #region STATES
        // Below are the three state functions. Each one is called based on the name of the state,
        // so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
        // Jump_SuperUpdate()
        void Idle_EnterState()
        {
            Debug.Log("IDLE");
            grounded = true;
            //anim.SetBool(GameConstants.ANIM_GROUNDED, grounded);
            EnableShadow(true);
        }

        void Idle_SuperUpdate()
        {
            if (Input.Current.JumpInput)
            {
                currentState = PlayerStates.Jump;
                return;
            }

            if (!MaintainingGround)
            {
                currentState = PlayerStates.Fall;
                return;
            }

            if (Input.Current.MoveInput != Vector3.zero)
            {
                currentState = PlayerStates.Walk;
                return;
            }

            if (Input.Current.AttackInput)
            {
                currentState = PlayerStates.Attack_Melee;
                return;
            }

            if (Input.Current.MagicInput)
            {
                currentState = PlayerStates.Attack_Magic;
                return;
            }

            // Apply friction to slow us to a halt
            moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, walkDecceleration * Time.deltaTime);
        }

        void Walk_EnterState()
        {
            walking = true;
           // anim.SetBool(GameConstants.ANIM_WALKING, walking);
        }

        void Walk_SuperUpdate()
        {
            if (Input.Current.JumpInput)
            {
                currentState = PlayerStates.Jump;
                return;
            }

            if (!MaintainingGround)
            {
                currentState = PlayerStates.Fall;
                return;
            }

            if(Input.Current.MoveInput != Vector3.zero)
            {
                moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement * speed, walkAcceleration * Time.deltaTime);
            }
            else
            {
                currentState = PlayerStates.Idle;
                return;
            }

            // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
            //var smoothedMovementFactor = (currentState.ToString() == PlayerStates.Jump.ToString()) ? groundDamping : inAirDamping; // how fast do we change direction?
            //float runSpeed = 5.0f; //Get from StatSystem
            //velocity.x = Mathf.Lerp(velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);

        }

        void Walk_ExitState()
        {
            walking = false;
           // anim.SetBool(GameConstants.ANIM_WALKING, walking);
        }

        void Jump_EnterState()
        {
            moveDirection.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
            grounded = false;
            //moveDirection += Controller.up * CalculateJumpSpeed(jumpHeight, gravity);
            //anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }

        void Jump_SuperUpdate()
        {
            if (MaintainingGround)
            {
                currentState = PlayerStates.Idle;
                return;
            }
            //anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);  
        }
        
        void Fall_EnterState()
        {
            grounded = false;
        }

        void Fall_SuperUpdate()
        {
            if (MaintainingGround)
            {
                currentState = PlayerStates.Idle;
                return;
            }

            moveDirection -= Vector3.up * gravity * Time.deltaTime;
            //anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }
        #endregion
    }
}