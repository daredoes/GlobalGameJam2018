using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral.ControlSystem
{
    public abstract class ControllerStateMachine : StateMachine
    {
        [SerializeField]
        protected Transform characterSprites;
        [SerializeField]
        protected GameObject shadow;
        [SerializeField]
        private SuperCharacterController _controller;
        [SerializeField]
        private ParticleSystem _feetParticles;
        [SerializeField]
        private Collider hitBoxCollider;

        public DamageType currentDamageType;
        public Vector3 damageDirection;

        protected Transform _trans;
        protected Animator _anim;
        protected Entity _entity;
        protected InventoryManager _inventoryManager;
        protected WeaponManager _weaponManager;
        protected StatCollection _statCollection;

        protected Vector3 moveDirection;
        #region VARS
        [SerializeField]
        protected float _mass = 1;
        [SerializeField]
        protected bool _grounded = true;
        [SerializeField]
        protected bool _facingRight = true;
        [SerializeField]
        protected bool _canMove = true;
        [SerializeField]
        protected bool _walking = false;
        [SerializeField]
        protected bool _blocking = false;
        [SerializeField]
        protected bool _attacking_melee = false;
        [SerializeField]
        protected bool _attacking_magic = false;
        [SerializeField]
        protected bool _throwing = false;
        [SerializeField]
        private bool isTakingDamage = false;
        [SerializeField]
        private bool isHit = false;
        private bool isFlinching = false;
        [SerializeField]
        private float flinchTime = 0.0f;

        [SerializeField]
        protected float _baseSpeed = 8.0f;
        [SerializeField]
        protected float walkAcceleration = 30.0f;
        [SerializeField]
        protected float walkDecceleration = 30.0f;
        [SerializeField]
        protected float jumpAcceleration = 10.0f;
        [SerializeField]
        protected float jumpHeight = 4.0f;
        [SerializeField]
        protected float gravity = 20.0f;
        protected Vector3 impact = Vector3.zero;


        protected float _attackThreshHold = 2f;
        protected float _timeSinceLastAttack;
        protected int _lastAttack = -1;
        protected int _currentAttack = 0;
        protected int _currentCombo;
        protected int _comboThreshHold = 5;

        protected bool stunSet = false;
        protected float _stunStart = 0f;
        protected float _stunTimeAfterKnockBack = 0.5f;
        #endregion

        #region PROPERTIES
        public SuperCharacterController Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = GetComponent<SuperCharacterController>();
                }
                return _controller;
            }
        }

        protected ParticleSystem feetParticles
        {
            get
            {
                if (_feetParticles == null)
                {
                    _feetParticles = GetComponent<ParticleSystem>();
                }
                return _feetParticles;
            }
        }

        public new Transform transform
        {
            get
            {
                if (_trans == null)
                {
                    _trans = GetComponent<Transform>();
                }
                return _trans;
            }
        }

        public Animator anim
        {
            get
            {
                if (_anim == null)
                {
                    _anim = GetComponentInChildren<Animator>();
                }
                return _anim;
            }
        }

        public Entity entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = GetComponent<Entity>();
                }
                return _entity;
            }
        }

        public InventoryManager inventoryManager
        {
            get
            {
                if (_inventoryManager == null)
                {
                    _inventoryManager = GetComponent<InventoryManager>();
                }
                return _inventoryManager;
            }
        }

        public StatCollection statCollection
        {
            get
            {
                if (_statCollection == null)
                {
                    _statCollection = GetComponent<StatCollection>();
                }
                return _statCollection;
            }
        }

        public WeaponManager weaponManager
        {
            get
            {
                if (_weaponManager == null)
                {
                    _weaponManager = GetComponent<WeaponManager>();
                }
                return _weaponManager;
            }
        }

        public Collider HitBox
        {
            get
            {
                if (hitBoxCollider == null)
                {
                    Debug.LogError("NO HIT BOX COLLIDER SET");
                }
                return hitBoxCollider;
            }
        }

        protected float mass
        {
            get
            {
                return _mass;
            }
        }

        protected float speed
        {
            get
            {
                //we can probably subtract a percentage of the inventory weight on here
                return statCollection.MoveSpeed * _baseSpeed;
            }
        }

        public bool grounded
        {
            get
            {
                return _grounded;
            }
            protected set
            {
                _grounded = value;
            }
        }

        protected bool AcquiringGround
        {
            get
            {
                return Controller.currentGround.IsGrounded(false, 0.01f);
            }
        }

        protected bool MaintainingGround
        {
            get
            {
                return Controller.currentGround.IsGrounded(true, 0.5f);
            }
        }

        public bool canMove
        {
            get
            {
                return _canMove;
            }
            protected set
            {
                _canMove = value;
            }
        }

        public bool facingRight
        {
            get
            {
                return _facingRight;
            }
            protected set
            {
                _facingRight = value;
            }
        }

        public bool walking
        {
            get
            {
                return _walking;
            }
            protected set
            {
                _walking = value;
            }
        }

        public bool blocking
        {
            get
            {
                return _blocking;
            }
            protected set
            {
                _blocking = value;
            }
        }


        public bool attacking_melee
        {
            get
            {
                return _attacking_melee;
            }
            set
            {
                _attacking_melee = value;
            }
        }

        public bool attacking_magic
        {
            get
            {
                return _attacking_magic;
            }
            set
            {
                _attacking_magic = value;
            }
        }

        public bool throwing
        {
            get
            {
                return _throwing;
            }
            set
            {
                _throwing = value;
            }
        }

        protected float timeSinceLastAttack
        {
            get
            {
                return _timeSinceLastAttack;
            }
            set
            {
                _timeSinceLastAttack = value;
            }
        }

        public int currentAttack
        {
            get
            {
                return _currentAttack;
            }
            set
            {
                _currentAttack = value;
            }
        }


        public int lastAttack
        {
            get
            {
                return _lastAttack;
            }
            set
            {
                _lastAttack = value;
            }
        }

        public int currentCombo
        {
            get
            {
                return _currentCombo;
            }
            set
            {
                _currentCombo = value;
            }
        }

        public int comboThreashold
        {
            get
            {
                return _comboThreshHold;
            }
            set
            {
                _comboThreshHold = value;
            }
        }

        public bool IsTakingDamage
        {
            get
            {
                return isTakingDamage;
            }

            set
            {
                isTakingDamage = value;
            }
        }

        public bool IsHit
        {
            get
            {
                return isHit;
            }

            set
            {
                isHit = value;
            }
        }

        public bool IsFlinching
        {
            get
            {
                return isFlinching;
            }
            set
            {
                isFlinching = value;
            }
        }

        public float FlinchTime
        {
            get
            {
                return flinchTime;
            }

            set
            {
                flinchTime = value;
            }
        }

        public Vector3 Impact
        {
            get
            {
                return impact;
            }

            set
            {
                impact = value;
            }
        }
        #endregion

        #region METHODS
        protected virtual void EmitLandedParticles(int amt = 5)
        {
            feetParticles.Emit(amt);
        }

        /// <summary>
        /// Initialize Character Controller
        /// </summary>
        public virtual void Initialize()
        {
            IsActive = true;
        }

        /// <summary>
        /// Enables and disables the character's shadow sprite as well as the feet dust particle system
        /// </summary>
        /// <param name="enable"></param>
        protected virtual void EnableShadow(bool enable)
        {
            shadow.SetActive(enable);
        }

        /// <summary>
        /// Enables and disables the character's dust particles
        /// </summary>
        /// <param name="enable"></param>
        protected virtual void EnableDust(bool enable)
        {
            if (enable)
            {
                if (!feetParticles.isPlaying)
                {
                    feetParticles.Play();
                }
            }
            else
            {
                if (!feetParticles.isStopped)
                {
                    feetParticles.Stop();
                }
            }
        }

        /// <summary>
        /// Calculate the initial velocity of a jump based off gravity and desired maximum height attained
        /// </summary>
        /// <param name="jumpHeight"></param>
        /// <param name="gravity"></param>
        /// <returns> the character's Jump Speed</returns>
        protected virtual float CalculateJumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }

        protected virtual float CalculateKnockBackSpeed(float force, float gravity)
        {
            return Mathf.Sqrt(3 * force * gravity);
        }

        /// <summary>
        /// Switches the character's primary weapon to the next available primary
        /// </summary>
        protected virtual void SwitchPrimary()
        {
            if (!inventoryManager.PrimarySlotEquipped)
            {
                if (inventoryManager.checkForWeapon(true))
                {
                    weaponManager.primaryRenderer.gameObject.SetActive(false);
                    inventoryManager.switchWeapon(true);
                    anim.SetTrigger(GameConstants.ANIM_SWITCH_PRIMARY);
                }
            }
            else
            {
                weaponManager.primaryRenderer.gameObject.SetActive(false);
                inventoryManager.switchWeapon(true);
                anim.SetTrigger(GameConstants.ANIM_SWITCH_PRIMARY);
            }
        }

        /// <summary>
        /// Switches the character's secondary weapon to the next available secondary
        /// </summary>
        protected virtual void SwitchSecondary()
        {
            if (!inventoryManager.SecondarySlotEquipped)
            {
                if (inventoryManager.checkForWeapon(false))
                {
                    weaponManager.primaryRenderer.gameObject.SetActive(false);
                    inventoryManager.switchWeapon(false);
                    anim.SetTrigger(GameConstants.ANIM_SWITCH_SECONDARY);
                }
            }
            else
            {
                weaponManager.primaryRenderer.gameObject.SetActive(false);
                inventoryManager.switchWeapon(false);
                anim.SetTrigger(GameConstants.ANIM_SWITCH_SECONDARY);
            }
        }

        /// <summary>
        /// Switches the character's projectile to the next available projectile
        /// </summary>
        protected void SwitchProjectile()
        {
            inventoryManager.switchProjectile();
        }

        /// <summary>
        /// Switches the character's throwable weapon to the next available throwable weapon
        /// </summary>
        protected void SwitchThrowable()
        {
            inventoryManager.switchThrowable();
        }

        /// <summary>
        /// Flips the character's sprites
        /// </summary>
        protected virtual void Flip()
        {
            facingRight = !facingRight;
            Vector3 temp = characterSprites.transform.localScale;
            temp.x *= -1;
            characterSprites.transform.localScale = temp;
            ParticleSystem.ShapeModule shape = feetParticles.shape;
            temp = feetParticles.shape.rotation;
            temp.y *= -1;
            shape.rotation = temp;
            OnFlip();
        }

        /// <summary>
        /// Do a melee attack
        /// </summary>
        protected virtual void Attack()
        {
            if (timeSinceLastAttack <= _attackThreshHold)
            {
                if (currentCombo > comboThreashold)
                {
                    //CAN USE SPECIAL - TRUE
                }
                currentCombo++;
                currentAttack = lastAttack == 0 ? (Random.Range(0, 100) % 2 == 0 ? 1 : Random.Range(0, 4)) : Random.Range(0, 4);
                if (currentAttack == 1 && lastAttack != 0)
                {
                    currentAttack = 0;
                }
            }
            else
            {
                currentAttack = Random.Range(0, 4);
                if (currentAttack == 1)
                {
                    currentAttack = 0;
                }
                currentCombo = 0;
            }
            anim.SetTrigger(GameConstants.ANIM_ATTACK);
            lastAttack = currentAttack;
            timeSinceLastAttack = 0f;
        }

        /// <summary>
        /// Do a special attack
        /// </summary>
        protected virtual void Magic()
        {
            //TODO: IMPLEMENT SPECIAL ATTACK and remove from current magic stat
            Debug.Log("[PlayerMachine]: MAGIC ATTACK!");
            anim.SetTrigger(GameConstants.ANIM_MAGIC);
            attacking_magic = false;
        }

        public abstract void Hit(int damage, DamageType dType, Vector3 direction);

        protected abstract void ApplyDamageType(bool forceKnockBack = false);
        #endregion

        #region EVENT HANDLERS
        public virtual void OnFlip()
        {
            // Called when character Flips
        }
        #endregion

    }
}
