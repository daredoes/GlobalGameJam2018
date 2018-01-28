﻿using System.Collections;
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
            Dash,
            Capture,
            Stun,// juggle / flinch state
            Attack_Melee,
            Attack_Magic,
            Throw,
            Fall
        }

        #region VARS
        
        bool previouslyOnGround;
        float groundDamping = 20f;
        float inAirDamping = 5f;
        //Replace this with 
        public float absorptionTime = 5.0f;
        public float leapLength = 5.0f;

        float timeLeftToAbsorb = 0;


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

                    //WOuld've been good way to avoidmaking method to tell player that got hit was subscribign event but guess not good
                    // 
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
            //_controller.onControllerCollidedEvent += (RaycastHit2D hit) => { if (hit.transform.gameObject.CompareTag("Virus")){ Debug.Log("yo"); } };



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
            if (Input.Current.JumpInput)
            {
                currentState = PlayerStates.Jump;
                return;
            }

            if (Input.Current.AttackInput || UnityEngine.Input.GetKeyDown(KeyCode.Q))
            {
                currentState = PlayerStates.Dash;
                return;
            }

            


            if (!IsGrounded)
            {
                currentState = PlayerStates.Fall;
                return;
            }

            if (Input.Current.MoveInput != Vector3.zero)
            {
                currentState = PlayerStates.Walk;
                return;
            }

            // Apply friction to slow us to a halt
            moveDirection.y = 0; //Vector3.MoveTowards(moveDirection, Vector3.zero, walkDecceleration * Time.deltaTime);
        }
        
        void Walk_EnterState()
        {
            Debug.Log("[Player Machine]: WALK");
            walking = true;
            anim.SetBool("WALKING", walking);
            anim.SetFloat("H_SPEED", Mathf.Abs(moveDirection.x));
        }

        void Walk_SuperUpdate()
        {
            if (Input.Current.JumpInput)
            {
                currentState = PlayerStates.Jump;
                return;
            }

            if (!IsGrounded)
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
            moveDirection.y += CalculateJumpSpeed(jumpHeight, gravity);
            grounded = false;
            anim.SetBool("GROUND", grounded);
        }

        void Jump_SuperUpdate()
        {
            if (IsGrounded)
            {
                currentState = PlayerStates.Idle;
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
                currentState = PlayerStates.Idle;
                return;
            }

            moveDirection -= Vector3.up * gravity * Time.deltaTime;
            Debug.Log("[Player Machine]: " + moveDirection);
            //anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }


        void Dash_EnterState()
        {   
         
            Debug.Log("Dash started");  

            moveDirection.y += CalculateJumpSpeed(jumpHeight / 3, gravity);
           
            grounded = false;
        }


        //This will be checking to transition into capture state or throwing
        void Dash_SuperUpdate()
        {

            Debug.Log("Dashing");

            int direction = (moveDirection.x < 0) ? -1 : 1;
            moveDirection.x += speed * Time.deltaTime * 2 * direction;


            if (IsGrounded)
            {
                currentState = PlayerStates.Idle;
                return;
            }
            
        }
        

        //Only enters this state if current state dash and collides with something
        void Capture_EnterState()
        {

            Debug.Log("Begun capture");
            timeLeftToAbsorb = absorptionTime;
        }

        void Capture_SuperUpdate()
        {

            Debug.Log("capturing");
            if (Input.Current.ThrowInput)
            {
                currentState = PlayerStates.Throw;
                return;
            }

            if (timeLeftToAbsorb >= 0)
            {
                timeLeftToAbsorb -= Time.deltaTime;
            }
            else
            {
                Heal();
                //Add to stats the bonus from capturing virus
                currentState = PlayerStates.Idle;
                return;
            }
        }

        void Heal()
        {

        }
        #endregion

        public void HitVirus()
        {

            if (currentState.Equals(PlayerStates.Dash))
            {
                currentState = PlayerStates.Capture;
            }

        }
    }
}