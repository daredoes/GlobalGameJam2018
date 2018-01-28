using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;

namespace Viral.ControlSystem
{
    public class AiMachine : ControllerStateMachine
    {
        enum States
        {
            Idle,
            Walk,
            Jump,
            Dash,
            Block,
            Juggle, // juggle / flinch state
            Attack_Melee,
            Attack_Magic,
            Throw,
            Fall
        }

        #region VARS
        bool previouslyOnGround;
        float groundDamping = 20f;
        float inAirDamping = 5f;
        [SerializeField]
        private float dashCooldown = 0.5f;
        float dashTime = 0f;


        [SerializeField]
        private NoInputController _input;
        private CharacterController2D _controller;
        #endregion

        #region PROPERTIES
        public NoInputController Input
        {
            get
            {
                if (_input == null)
                {
                    _input = GetComponent<NoInputController>();
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

        public new bool IsGrounded
        {
            get
            {
                return Controller.isGrounded;
            }
        }

        public new bool CanDash{
            get
            {
                if (Time.time - dashTime >= dashCooldown)
                {
                    return true;
                }
                return false;
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

        public void FlipSprite(){
            Flip();
        }

        public override void Initialize()
        {
            //inventoryManager.OnItemEncounter += ItemEncountered;
            //inventoryManager.OnItemLeft += ItemLeft;
            currentState = States.Idle;
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
            var smoothedMovementFactor = _controller.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
            moveDirection.x = Mathf.Lerp(moveDirection.x, LocalMovement.x * speed, Time.deltaTime * smoothedMovementFactor);
            moveDirection.y -= gravity * Time.deltaTime;
            Controller.move(moveDirection * Time.deltaTime);
            moveDirection = Controller.velocity;
        }

        #region STATES
        // Below are the three state functions. Each one is called based on the name of the state,
        // so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
        // Jump_SuperUpdate()
        void Idle_EnterState()
        {
            Debug.Log("[Player Machine]: IDLE");
            grounded = true;
            anim.SetBool("GROUND", grounded);
            EnableShadow(true);
            moveDirection.y = 0;
        }

        void Idle_SuperUpdate()
        {
            if (Input.Current.JumpInput != Vector3.zero)
            {
                currentState = States.Jump;
                return;
            }

            Debug.Log("[AI Machine]: " + IsGrounded);
            if (!IsGrounded)
            {
                currentState = States.Fall;
                return;
            }
            if (CanDash && Input.Current.DashInput != Vector3.zero)
            {
                currentState = States.Dash;
                return;
            }

            if (Input.Current.MoveInput != Vector3.zero)
            {
                currentState = States.Walk;
                return;
            }

            // Apply friction to slow us to a halt
            moveDirection.y = 0; //Vector3.MoveTowards(moveDirection, Vector3.zero, walkDecceleration * Time.deltaTime);
        }
        
        void Walk_EnterState()
        {
            Debug.Log("[AI Machine]: WALK");
            walking = true;
            anim.SetBool("WALKING", walking);
            anim.SetFloat("H_SPEED", Mathf.Abs(moveDirection.x));
        }

        void Walk_SuperUpdate()
        {
            if (Input.Current.JumpInput != Vector3.zero)
            {
                currentState = States.Jump;
                return;
            }

            if (!IsGrounded)
            {
                currentState = States.Fall;
                return;
            }
            if (CanDash && Input.Current.DashInput != Vector3.zero)
            {
                currentState = States.Dash;
                return;
            }

            if(Input.Current.MoveInput != Vector3.zero)
            {
                moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement * speed, walkAcceleration * Time.deltaTime);
            }
            else
            {
                currentState = States.Idle;
                return;
            }

            anim.SetFloat("H_SPEED", Mathf.Abs(moveDirection.x));

            // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
            //var smoothedMovementFactor = (currentState.ToString() == PlayerStates.Jump.ToString()) ? groundDamping : inAirDamping; // how fast do we change direction?
            //float runSpeed = 5.0f; //Get from StatSystem
            //velocity.x = Mathf.Lerp(velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);
        }

        void Walk_ExitState()
        {
            walking = false;
            anim.SetBool("WALKING", walking);
            anim.SetFloat("H_SPEED", Mathf.Abs(moveDirection.x));
        }

        void Jump_EnterState()
        {
            Debug.Log("[Player Machine]: JUMP");
            moveDirection.y += CalculateJumpSpeed(Input.Current.JumpInput.y, Input.Current.JumpInput.z);
            moveDirection.x += Input.Current.JumpInput.x;
            grounded = false;
            anim.SetBool("GROUND", grounded);
        }

        void Jump_SuperUpdate()
        {
            if (IsGrounded)
            {
                currentState = States.Idle;
                return;
            }
            if (CanDash && Input.Current.DashInput != Vector3.zero)
            {
                currentState = States.Dash;
                return;
            }
            anim.SetFloat("V_SPEED", moveDirection.y);  
        }

        void Dash_EnterState()
        {
            Debug.Log("[Player Machine]: DASH");
            dashTime = Time.time + dashCooldown;
            moveDirection.y += CalculateJumpSpeed(Input.Current.DashInput.y, Input.Current.DashInput.z);
            int direction = facingRight ? 1 : -1;
            moveDirection.x += Input.Current.DashInput.x * direction;
            grounded = false;
            anim.SetBool("GROUND", grounded);

        }

        void Dash_SuperUpdate()
        {
            if (IsGrounded)
            {
                currentState = States.Idle;
                return;
            }
            if (CanDash && Input.Current.DashInput != Vector3.zero)
            {
                currentState = States.Dash;
                return;
            }
            anim.SetFloat("V_SPEED", moveDirection.y);
        }
        
        void Fall_EnterState()
        {
            Debug.Log("[Player Machine]: FALL");
            grounded = false;
            anim.SetBool("GROUND", grounded);
        }

        void Fall_SuperUpdate()
        {
            if (IsGrounded)
            {
                currentState = States.Idle;
                return;
            }

            moveDirection -= Vector3.up * gravity * Time.deltaTime;
            Debug.Log("[Player Machine]: " + moveDirection);
            //anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }
        #endregion
    }
}