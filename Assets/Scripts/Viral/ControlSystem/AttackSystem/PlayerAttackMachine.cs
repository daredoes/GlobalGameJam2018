using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Viral.ControlSystem.AttackSystem
{
    public class PlayerAttackMachine : ControllerStateMachine
    {

        //What I could do is instead of put ammo types along with it is maybe PlayerAttack types not just melee/ranged but just different weapons period?
        //Cause if do other way where have another set that won't be used if doing melee, well would be alot of dupe code though especially with ranged stuff, k just another enum.
        //No time for trivial changes in design like that.
        enum PlayerAttackType
        {
            Melee,
            Ranged
        }

        enum AmmoType
        {
        }

        //Some dupe code, I could continue to handle input in PlayerMachine, but already alot there
        private InputController _input;
        //Or would ammo be a completely different machine? Cause think that's overkill lol.
        AmmoType ammoType;


        public float reloadTime = 5.0f;
        float reloadTimeLeft;

        public float damageAmpCap = 2.0f;
        public float speedAmpCap = 1.0f;
        float damageAmp;
        float speedAmp;

        GameObject bulletCharging;

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

        void Start()
        {
            Initialize();

        }


        public override void Initialize()
        {
            base.Initialize();

            currentState = PlayerAttackType.Ranged;
           // ammoType = AmmoType.QuickShot;

        }

        protected override void EarlyGlobalSuperUpdate()
        {
            base.EarlyGlobalSuperUpdate();           
        }

        protected override void LateGlobalSuperUpdate()
        {
            base.LateGlobalSuperUpdate();
            //Replace with controller input later, using unity input for testing
            if (UnityEngine.Input.GetKeyDown(KeyCode.LeftShift))
            {
                currentState = (currentState.Equals(PlayerAttackType.Melee)) ? PlayerAttackType.Ranged : PlayerAttackType.Melee;   
            }

        }

        void Melee_EnterState()
        {
            //For bringing out weapon
            Debug.Log("Now melee mode");
        }

        void Melee_SuperUpdate()
        {

        }

        void Melee_ExitState()
        {
            //For putting away weapon
        }

        void Ranged_EnterState()
        {
            Debug.Log("Now ranged mode");

            
            
        }
        void Ranged_SuperUpdate()
        {
            //In here will be swapping ammo based on input / things absorbed, need way to connect this to what Player has been able to shoot

            //Will load in from pool so no need to instantiate and do resouces . load on it

            //Attack Input is all on pressing, not up, down, or held so just to finish will just use Unity Engine Input, it makes harder for phone
            //but that's minor
            //if (Input.Current.AttackInput && reloadTimeLeft <= 0)

            if (reloadTimeLeft <= 0) {


                if (UnityEngine.Input.GetKey(KeyCode.F))
                {

                    //Begin charging
                    damageAmp += Time.deltaTime;
                    //Speed increases by 10% of what currently is everytime
                    speedAmp += (speedAmp + Time.deltaTime) * 0.1f;

                    bulletCharging = Instantiate(Resources.Load(string.Format("Prefabs/PlayerAmmo/{0}", ammoType.ToString())) as GameObject);
                    //PooledObject ammoPrefab = PoolManager.instance.Acquire(ammoType.toString());
                    reloadTimeLeft = reloadTime;

                    bulletCharging.transform.parent = this.transform;
                    bulletCharging.transform.localPosition = Vector3.zero;

                    bulletCharging.SetActive(true);

                }

                if (UnityEngine.Input.GetKeyUp(KeyCode.F))
                {
                    bulletCharging.GetComponent<Ammo>().Shoot(damageAmp, speedAmp);
                    damageAmp = 0;
                    speedAmp = 0;
                }


                if (bulletCharging != null)
                {
                    bulletCharging.transform.Rotate(new Vector3(0, 0, GetComponent<PlayerMachine>().facingRight ? -90 : 90));
                }

                if (reloadTimeLeft > 0)
                {
                    reloadTimeLeft -= Time.deltaTime;
                }
            }
        }

        void Ranged_ExitState()
        {
        }

    }
}