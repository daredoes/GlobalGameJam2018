using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prime31;
using Viral.StatSystem;

namespace Viral.ControlSystem
{
    public class PlayerMachine : ControllerStateMachine
    {
        enum PlayerStates
        {
            Idle,
            Walk,
            Jump,
            Capture,
            Stun,
            Dash,
            Slam,
            Attack_Melee,
            Attack_Magic,
            Throw,
            Fall
        }

        #region VARS
        
        bool previouslyOnGround;
        float groundDamping = 20f;
        float inAirDamping = 5f;

        float dashCooldown = 0.01f;
        float dashTime = 0f;

        float slamCooldown = 0.01f;
        float slamTime = 0f;

        public float StunTime 
        {

            get{
                return 2.0f;
            }

        }
        public float absorptionTime = 5.0f;

        float timeStunned = 0;
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

        public bool CanDash{
            get
            {
                if(Time.time - dashTime >= dashCooldown){
                    return true;
                }
                return false;
            }
        }

        public bool CanSlam
        {
            get
            {
                if (Time.time - slamTime >= slamCooldown)
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
                if (timeStunned <= 0)
                {
                    if (Input.Current.MoveInput.x != 0)
                    {
                        local += transform.right * Input.Current.MoveInput.x;
                    }
                    if (Input.Current.MoveInput.z != 0)
                    {
                        local += transform.forward * Input.Current.MoveInput.z;
                    }
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
            base.EarlyGlobalSuperUpdate();

            // Put any code in here you want to run BEFORE the state's update function.
            // This is run regardless of what state you're in
            if (timeStunned <= 0)
            {

                //Flip the character
                if ((Input.Current.MoveInput.x > 0 && !facingRight) || (Input.Current.MoveInput.x < 0 && facingRight)) { Flip(); }
            }
        }

        protected override void LateGlobalSuperUpdate()
        {
            // Put any code in here you want to run AFTER the state's update function.
            // This is run regardless of what state you're in
            // Move the player by our velocity every frame

            base.LateGlobalSuperUpdate();
            Debug.Log(currentState.ToString());
            if (currentState.Equals(PlayerStates.Stun)) { }
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
                currentState = PlayerStates.Jump;
                return;
            }

            if (CanDash && Input.Current.DashInput != Vector3.zero){
                currentState = PlayerStates.Dash;
                return;
            }

            Debug.Log("[Player Machine]: " + IsGrounded);
            if (!IsGrounded)
            {
                currentState = PlayerStates.Dash;
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
            if (Input.Current.JumpInput != Vector3.zero)
            {
                currentState = PlayerStates.Jump;
                return;
            }
            if (CanDash && Input.Current.DashInput != Vector3.zero)
            {
                currentState = PlayerStates.Dash;
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
            moveDirection.y += CalculateJumpSpeed(Input.Current.JumpInput.y, Input.Current.JumpInput.z);
            moveDirection.x += Input.Current.JumpInput.x;
            grounded = false;
            anim.SetBool("GROUND", grounded);
        }

        void Jump_SuperUpdate()
        {
            if (Input.Current.JumpInput != Vector3.zero){
                currentState = PlayerStates.Jump;
                return;
            }
            if (IsGrounded)
            {
                currentState = PlayerStates.Idle;
                return;
            }
            if (CanSlam && Input.Current.SlamInput != Vector3.zero)
            {
                currentState = PlayerStates.Slam;
                return;
            }
            if (CanDash && Input.Current.DashInput != Vector3.zero)
            {
                currentState = PlayerStates.Dash;
                return;
            }
            anim.SetFloat("V_SPEED", moveDirection.y);  
        }

        void Slam_EnterState()
        {
            slamTime = Time.time + slamCooldown;
            moveDirection.y += CalculateJumpSpeed(Input.Current.SlamInput.y, Input.Current.SlamInput.z);
            grounded = false;
            anim.SetBool("GROUND", grounded);
        }
        void Slam_SuperUpdate()
        {
            if (IsGrounded)
            {
                currentState = PlayerStates.Idle;
                return;
            }
            anim.SetFloat("V_SPEED", moveDirection.y);
        }

        void Dash_EnterState(){
            Debug.Log("[Player Machine]: DASH");
            dashTime = Time.time + dashCooldown;
            moveDirection.y += CalculateJumpSpeed(Input.Current.DashInput.y, Input.Current.DashInput.z);
            int direction = facingRight ? 1 : -1;
            moveDirection.x += Input.Current.DashInput.x * direction;
            grounded = false;
            anim.SetBool("GROUND", grounded);
            
        }
        void Dash_SuperUpdate(){
            if (IsGrounded)
            {
                currentState = PlayerStates.Idle;
                return;
            }
            if (CanSlam && Input.Current.SlamInput != Vector3.zero)
            {
                currentState = PlayerStates.Slam;
                return;
            }
            if (CanDash && Input.Current.DashInput != Vector3.zero)
            {
                currentState = PlayerStates.Dash;
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
            //anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
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

            if (timeLeftToAbsorb > 0)
            {
                //Works for entering state.
                if (UnityEngine.Input.GetKeyDown(KeyCode.R))
                {
                    
                    currentState = PlayerStates.Stun;
                    timeLeftToAbsorb = 0;
                    return;
                }

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

        void Capture_ExitState()
        {
            if (timeLeftToAbsorb > 0)
            {
                currentState = PlayerStates.Stun;
                timeLeftToAbsorb = 0;
            }
        }

        //ToDo: On Capture should call method to nab the enemy away,and on Capture exit releaseo
        /*
         * 
         * 
         * 
         * 
         * */

        void Stun_EnterState()
        {

            Debug.Log("Entered state");
            timeStunned = StunTime;
            StartCoroutine(fall());
        }

        IEnumerator fall()
        {
            while (moveDirection.y > 0)
            {
                moveDirection.y -= Time.deltaTime * gravity;
                yield return new WaitForEndOfFrame();
            }
        }

        void Stun_SuperUpdate()
        {
            if (timeStunned > 0)
            {
                timeStunned -= Time.deltaTime;
                
                moveDirection = Vector3.zero;
            }
            else
            {
                currentState = PlayerStates.Idle;
                
            }

        }
        
        void Heal(int healAmount = 2)
        {
            ((StatVital)statCollection[StatType.Health]).Value += healAmount;
        
        }

      

        void TakeDamage(float dmgAmount, ControlSystem.AttackSystem.DamageType type, Vector3 direction)
        {
            ((StatVital)statCollection[StatType.Health]).Value -= (int)dmgAmount;



            switch (type)
            {

                case ControlSystem.AttackSystem.DamageType.FORCE_STUN:
                    currentState = PlayerStates.Stun;
                    return;
                    
                case ControlSystem.AttackSystem.DamageType.MELEE:
                    if (currentState.Equals(PlayerStates.Capture))
                    {
                        currentState = PlayerStates.Stun;
                        return;
                    }
                    break;
                case ControlSystem.AttackSystem.DamageType.POISON:
                    break;
            }

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