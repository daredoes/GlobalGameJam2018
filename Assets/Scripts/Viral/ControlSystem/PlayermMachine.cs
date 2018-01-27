using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
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
        [SerializeField]
        private InputController _input;
        private GameObject _pickUpButton;
        private float timeSinceLastAerialAttack = 0f;
        private float aerialAttackThreshHold = 0.5f;
        private bool aerialAttackStarted = false;
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

        GameObject pickUpButton
        {
            get
            {
                if (_pickUpButton == null)
                {
                    //_pickUpButton = Mobile.MobileController.controller.buttons.PickUp.gameObject;
                }
                return _pickUpButton;
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

            if (Input.Current.ItemPickUp)
            {
                PickUpItem();
            }

            if (Input.Current.OpenInventory)
            {
                OpenInventory();
            }
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
            aerialAttackStarted = false;
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
            moveDirection = Vector3.MoveTowards(moveDirection, Vector3.zero, walkDecceleration * Controller.deltaTime);
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
                return;
            }

            if (Input.Current.BlockInput)
            {
                currentState = PlayerStates.Block;
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

            if (Input.Current.MoveInput != Vector3.zero)
            {
                //moveDirection = Vector3.MoveTowards(moveDirection, LocalMovement * speed, walkAcceleration * Controller.deltaTime);
            }
            else
            {
                currentState = PlayerStates.Idle;
                return;
            }


            EnableDust(grounded);
        }

        void Walk_ExitState()
        {
            walking = false;
            anim.SetBool(GameConstants.ANIM_WALKING, walking);
        }

        void Attack_Melee_EnterState()
        {
            if (IsHit)
            {
                ApplyDamageType();
                return;
            }
            if (!grounded)
            {
                moveDirection = Vector3.zero;
                attacking_melee = true;
                aerialAttackStarted = true;
                timeSinceLastAerialAttack = 0;
                anim.SetBool(GameConstants.ANIM_GROUNDED, false);
                anim.SetBool(GameConstants.ANIM_AERIAL, true);
                Attack();
            }
            else
            {
                moveDirection = Vector3.zero;
                attacking_melee = true;
                Attack();
            }
        }

        void Attack_Melee_SuperUpdate()
        {
            if (grounded)
            {
                if (attacking_melee)
                {
                    moveDirection = Vector3.zero;
                }
                else
                {
                    if (IsHit)
                    {
                        ApplyDamageType();
                        return;
                    }
                    if (Input.Current.AttackInput)
                    {
                        Attack_Melee_EnterState();
                        return;
                    }
                    currentState = PlayerStates.Idle;
                    return;
                }
                if (IsHit)
                {
                    ApplyDamageType();
                    return;
                }
            }
            else
            {
                if (attacking_melee)
                {
                    moveDirection -= Controller.up * (gravity * .1f) * Controller.deltaTime;
                }
                else
                {
                    if (Input.Current.AttackInput)
                    {
                        if (aerialAttackStarted)
                        {
                            if (timeSinceLastAerialAttack < aerialAttackThreshHold)
                            {
                                Attack_Melee_EnterState();
                                return;
                            }
                            else
                            {
                                Debug.Log("CAN'T DO IT");
                            }
                        }
                        else
                        {
                            Attack_Melee_EnterState();
                            return;
                        }
                    }
                    currentState = PlayerStates.Fall;
                    return;
                }
            }
        }

        void Attack_Magic_EnterState()
        {
            if (!IsActive) { return; }
            if (statCollection.GetStat<StatRegeneratable>(StatType.Magic).Value < 8)
            {
                currentState = PlayerStates.Idle;
                return;
            }
            moveDirection = Vector3.zero;
            attacking_magic = true;
            Magic();
        }

        void Attack_Magic_SuperUpdate()
        {
            moveDirection = Vector3.zero;
            if (!attacking_magic)
            {
                if (!MaintainingGround)
                {
                    currentState = PlayerStates.Fall;
                    return;
                }
                currentState = PlayerStates.Idle;
                return;
            }
        }

        void Throw_EnterState()
        {
            if (IsHit)
            {
                ApplyDamageType();
                return;
            }

            switch (inventoryManager.throwableInUse)
            {
                case InventoryManager.ThrowableInUse.Projectile:
                    if (inventoryManager.LauncherSlotEquipped && inventoryManager.ProjectileSlotEquipped)
                    {
                        if (inventoryManager.Projectile.StackSize > 0)
                        {
                            moveDirection = Vector3.zero;
                            throwing = true;
                            if (!grounded)
                            {
                                anim.SetBool(GameConstants.ANIM_GROUNDED, false);
                                anim.SetBool(GameConstants.ANIM_AERIAL, true);
                            }
                            anim.SetTrigger(GameConstants.ANIM_BOW);
                            return;
                        }
                    }
                    break;
                case InventoryManager.ThrowableInUse.Throwable:
                    if (inventoryManager.ThrowableSlotEquipped)
                    {
                        if (inventoryManager.Throwable.StackSize > 0)
                        {
                            moveDirection = Vector3.zero;
                            throwing = true;
                            if (!grounded)
                            {
                                anim.SetBool(GameConstants.ANIM_AERIAL, true);
                            }
                            anim.SetTrigger(GameConstants.ANIM_THROW);
                            return;
                        }
                    }
                    break;
            }
        }

        void Throw_SuperUpdate()
        {
            if (grounded)
            {
                if (throwing)
                {
                    moveDirection = Vector3.zero;
                }
                else
                {
                    if (IsHit)
                    {
                        ApplyDamageType();
                        return;
                    }

                    currentState = PlayerStates.Idle;
                    return;
                }
            }
            else
            {
                if (throwing)
                {
                    // Move down slower
                    moveDirection -= Controller.up * (gravity * .1f) * Controller.deltaTime;
                }
                else
                {
                    if (IsHit)
                    {
                        ApplyDamageType();
                        return;
                    }

                    currentState = PlayerStates.Fall;
                    return;
                }
            }
            if (IsHit)
            {
                ApplyDamageType();
                return;
            }

        }

        void Block_EnterState()
        {
            blocking = true;
            anim.SetBool(GameConstants.ANIM_BLOCKING, blocking);
        }

        void Block_SuperUpdate()
        {
            moveDirection = Vector3.zero;

            if (!Input.Current.BlockInput)
            {
                currentState = PlayerStates.Idle;
                return;
            }
        }

        void Block_ExitState()
        {
            blocking = false;
            anim.SetBool(GameConstants.ANIM_BLOCKING, blocking);
        }

        void Juggle_EnterState()
        {
            anim.SetBool("KnockBack", true);
            Controller.DisableClamping();
            Controller.DisableSlopeLimit();
            grounded = false;
            EnableShadow(grounded);
            EnableDust(grounded);
            moveDirection += Controller.up * CalculateKnockBackSpeed(2, gravity);
            anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);

            if (IsHit)
            {
                ApplyDamageType();
                return;
            }
        }

        void Juggle_SuperUpdate()
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

            if (IsHit)
            {
                ApplyDamageType();
                return;
            }

            planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, (damageDirection / 3f) * _baseSpeed, jumpAcceleration * Controller.deltaTime);
            verticalMoveDirection -= Controller.up * gravity * Controller.deltaTime;
            moveDirection = planarMoveDirection + verticalMoveDirection;
            anim.SetFloat(GameConstants.ANIM_VERTICAL_SPEEED, moveDirection.y);
        }

        void Juggle_ExitState()
        {
            if (entity.IsDead)
            {
                entity.FlashSprites();
                //leave in knockback state
            }
            else
            {
                anim.SetBool("KnockBack", false);
            }
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

            if (Input.Current.ThrowInput)
            {
                currentState = PlayerStates.Throw;
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

        /// <summary>
        /// Opens the Inventory UI
        /// </summary>
        void OpenInventory()
        {
            PlayerPanels.instance.Open(entity);
        }

        private void ItemEncountered(object sender = null, System.EventArgs e = null)
        {
            EnablePickUp();
        }

        private void ItemLeft(object sender = null, System.EventArgs e = null)
        {
            DisablePickUp();
        }

        void EnablePickUp()
        {
            pickUpButton.GetComponentInChildren<UnityEngine.UI.Text>().text = inventoryManager.currentItemObject.GetComponent<ItemCollectable>().MyItem.Name;
            pickUpButton.SetActive(true);
        }

        void DisablePickUp()
        {
            pickUpButton.SetActive(false);
        }

        void PickUpItem()
        {
            inventoryManager.PickUpItem();
        }
        #endregion
    }
}

*/