using System.Collections;
using System.Collections.Generic;
using Viral.StatSystem;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine;

namespace Viral.ControlSystem
{
    public abstract class ControllerStateMachine : StateMachine
    {
        [SerializeField]
        protected Transform characterSprites;
        [SerializeField]
        protected GameObject shadow;

        protected Transform _trans;
        protected Animator _anim;
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
        private bool isTakingDamage = false;
        [SerializeField]
        private bool isHit = false;
        
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

        protected bool stunSet = false;
        protected float _stunStart = 0f;
        protected float _stunTimeAfterKnockBack = 0.5f;
        #endregion

        #region PROPERTIES
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

        protected bool IsGrounded
        {
            get
            {
                return true;
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
        #endregion

        #region METHODS
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
        /// Calculate the initial velocity of a jump based off gravity and desired maximum height attained
        /// </summary>
        /// <param name="jumpHeight"></param>
        /// <param name="gravity"></param>
        /// <returns> the character's Jump Speed</returns>
        protected virtual float CalculateJumpSpeed(float jumpHeight, float gravity)
        {
            return Mathf.Sqrt(2 * jumpHeight * gravity);
        }
     
        public virtual void TakeDamage(float dmgAmount, ControlSystem.AttackSystem.DamageType type, Vector3 direction, Viral.ControlSystem.ControllerStateMachine attacker)
        {
            Debug.Log("Previous health" + ((StatVital)statCollection[StatType.Health]).Value);
            ((StatVital)statCollection[StatType.Health]).Value -= (int)dmgAmount;
            Debug.Log("I am hurt. New Health: " + ((StatVital)statCollection[StatType.Health]).Value);
        }

        /// <summary>
        /// Flips the character's sprites
        /// </summary>
        public virtual void Flip()
        {
            facingRight = !facingRight;
            Vector3 temp = characterSprites.transform.localScale;
            temp.x *= -1;
            characterSprites.transform.localScale = temp;
            //ParticleSystem.ShapeModule shape = feetParticles.shape;
            //temp = feetParticles.shape.rotation;
            //temp.y *= -1;
            //shape.rotation = temp;
            OnFlip();
        }
        #endregion

        #region EVENT HANDLERS
        public virtual void OnFlip()
        {
            // Called when character Flips
        }
        #endregion

    }
}
