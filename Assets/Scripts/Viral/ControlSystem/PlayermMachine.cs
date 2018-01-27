using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        float groundDamping = 20f;
        float inAirDamping = 5f;
        float gravity = -25f;
        Vector3 velocity;


        [SerializeField]
        private InputController _input;
       
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

        public override void Initialize()
        {
            //inventoryManager.OnItemEncounter += ItemEncountered;
            //inventoryManager.OnItemLeft += ItemLeft;
            currentState = PlayerStates.Idle;
            base.Initialize();
        }

        protected override void EarlyGlobalSuperUpdate()
        {
            // Put any code in here you want to run BEFORE the state's update function.
            // This is run regardless of what state you're in
            base.EarlyGlobalSuperUpdate();
            //Flip the character
            if ((Input.Current.FlipInput) || (Input.Current.MoveInput.x > 0 && !facingRight) || (Input.Current.MoveInput.x < 0 && facingRight)) { Flip(); }

          
        }

        protected override void LateGlobalSuperUpdate()
        {
            // Put any code in here you want to run AFTER the state's update function.
            // This is run regardless of what state you're in
            // Move the player by our velocity every frame
            base.LateGlobalSuperUpdate();
            //transform.position += moveDirection * Controller.deltaTime;
            //timeSinceLastAttack += Controller.deltaTime;
            //timeSinceLastAerialAttack += Controller.deltaTime;
        }

        #region STATES
        // Below are the three state functions. Each one is called based on the name of the state,
        // so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call
        // Jump_SuperUpdate()
        void Idle_EnterState()
        {
            if (IsHit)
            {
                ApplyDamageType();
                return;
            }
           // Controller.EnableSlopeLimit();
           // Controller.EnableClamping();
            grounded = true;
            //anim.SetBool(GameConstants.ANIM_GROUNDED, grounded);
            EnableShadow(true);
            //EnableDust(grounded);
        }

        void Idle_SuperUpdate()
        {
            if (IsHit)
            {
                ApplyDamageType();
                return;
            }

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

            if (Input.Current.ThrowInput)
            {
                currentState = PlayerStates.Throw;
                return;
            }

            if (Input.Current.BlockInput)
            {
                currentState = PlayerStates.Block;
                return;
            }

            // Apply friction to slow us to a halt
         //   moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, walkDecceleration * Controller.deltaTime);

        }

        void Walk_EnterState()
        {
            if (IsHit)
            {
                ApplyDamageType();
                return;
            }

            walking = true;
           // anim.SetBool(GameConstants.ANIM_WALKING, walking);
        }

        void Walk_SuperUpdate()
        {
           
            if (IsHit)
            {
                ApplyDamageType();
                return;
            }

            if (Input.Current.JumpInput)
            {
                currentState = PlayerStates.Jump;
                return;
            }

            if (!MaintainingGround)
            {
                currentState = PlayerStates.Fall;
            }
            else
            {
                velocity.y = 0;
            }
        
            //But input stuff shouldn't be here, so these in separate methods?
            if (Input.Current.MoveInput == Vector3.right)
            {
                normalizedHorizontalSpeed = 1;

                if (transform.localScale.x < 0f)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                //moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement * speed, walkAcceleration * Controller.deltaTime);
            }
            else if (Input.Current.MoveInput == -Vector3.right)
            {
                normalizedHorizontalSpeed = -1;

                if (transform.localScale.x > 0f)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

            }
            else if (Input.Current.MoveInput == Vector3.up)
            {
                if (currentState.ToString() != PlayerStates.Fall.ToString() && currentState.ToString() != PlayerStates.Jump.ToString())
                {
                    velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);
                }

            }
            else if (Input.Current.MoveInput == Vector3.zero)
            {
                currentState = PlayerStates.Idle;
                normalizedHorizontalSpeed = 0;
            }

            // apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
            var smoothedMovementFactor = (currentState.ToString() == PlayerStates.Jump.ToString()) ? groundDamping : inAirDamping; // how fast do we change direction?
            float runSpeed; //Get from StatSystem
            velocity.x = Mathf.Lerp(velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor);


            velocity.y += gravity * Time.deltaTime;
            transform.Translate(velocity, Space.World);
            // EnableDust(grounded);
        }

        void Walk_ExitState()
        {
            walking = false;
            anim.SetBool(GameConstants.ANIM_WALKING, walking);
        }

        void Jump_EnterState()
        {
            Controller.DisableClamping();
            Controller.DisableSlopeLimit();
            grounded = false;
            anim.SetBool(GameConstants.ANIM_GROUNDED, grounded);
            EnableShadow(grounded);
            EnableDust(grounded);
            moveDirection += Controller.up * CalculateJumpSpeed(jumpHeight, gravity);
            anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }

        void Jump_SuperUpdate()
        {
            Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(Controller.up, moveDirection);
            Vector3 verticalMoveDirection = moveDirection - planarMoveDirection;

            if (Vector3.Angle(verticalMoveDirection, Controller.up) > 90 && AcquiringGround)
            {
                moveDirection = planarMoveDirection;
                EmitLandedParticles();
                currentState = PlayerStates.Idle;
                return;
            }

            if (Input.Current.AttackInput)
            {
                currentState = PlayerStates.Attack_Melee;
                return;
            }

            if (Input.Current.MagicInput)
            {
                currentState = PlayerStates.Attack_Melee;
                return;
            }

            //TODO: IMPLEMENT ATTACK INPUT
            if (Input.Current.ThrowInput)
            {
                currentState = PlayerStates.Throw;
                return;
            }

            planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement * speed, jumpAcceleration * Controller.deltaTime);
            verticalMoveDirection -= Controller.up * gravity * Controller.deltaTime;
            moveDirection = planarMoveDirection + verticalMoveDirection;
            anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }

        void Fall_EnterState()
        {
            Controller.DisableClamping();
            Controller.DisableSlopeLimit();
            grounded = false;
            anim.SetBool(GameConstants.ANIM_GROUNDED, grounded);
            EnableShadow(grounded);
            EnableDust(grounded);
        }

        void Fall_SuperUpdate()
        {

            if (AcquiringGround)
            {
                moveDirection = Math3d.ProjectVectorOnPlane(Controller.up, moveDirection);
                EmitLandedParticles();
                currentState = PlayerStates.Idle;
                return;
            }

            

            moveDirection -= Controller.up * gravity * Controller.deltaTime;
            anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }
        #endregion

        #region METHODS
        public override void Hit(int damage, Vector3 direction)
        {
            damageDirection = direction;
            IsHit = true;
        }

        protected override void ApplyDamageType(bool forceKnockBack = false)
        {
            if (forceKnockBack)
            {
                currentDamageType = DamageType.KnockBack;
            }
            else
            {
                //make any changes to the current damage depending on the current state
                switch ((PlayerStates)currentState)
                {
                    case PlayerStates.Idle:
                        break;
                    case PlayerStates.Juggle:
                        currentDamageType = DamageType.KnockBack;
                        break;
                    case PlayerStates.Walk:
                        break;
                    case PlayerStates.Throw:
                    case PlayerStates.Attack_Magic:
                    case PlayerStates.Attack_Melee:
                        if (!grounded)
                        {
                            currentDamageType = DamageType.KnockBack;
                        }
                        break;
                    case PlayerStates.Jump: // automatic knockback to juggle state if hit in air
                    case PlayerStates.Fall:
                        if (currentDamageType == DamageType.Normal || currentDamageType == DamageType.Flinch)
                        {
                            currentDamageType = DamageType.KnockBack;
                        }
                        break;
                }
            }

            IsHit = false;
            //apply the damage type to character 
            switch (currentDamageType)
            {
                case DamageType.Normal:
                case DamageType.Flinch:
                    anim.SetTrigger("Hit");
                    currentState = PlayerStates.Idle;
                    return;
                case DamageType.KnockBack:
                    if ((PlayerStates)currentState == PlayerStates.Juggle)
                    {
                        Juggle_EnterState();
                        return;
                    }
                    else
                    {
                        currentState = PlayerStates.Juggle;
                        return;
                    }
                case DamageType.None:
                    Debug.Log("DO nothing");
                    return;
                default:
                    anim.SetTrigger("Hit");
                    currentState = PlayerStates.Idle;
                    return;
            }
        }

       
        #endregion
    }
}