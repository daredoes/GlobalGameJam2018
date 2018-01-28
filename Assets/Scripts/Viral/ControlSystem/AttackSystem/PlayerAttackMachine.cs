using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Viral.ControlSystem.AttackSystem
{
    //Didn't make this generic made it very specific for player
    public class PlayerAttackMachine : StateMachine
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
            DefaultAmmo
        }

#region Variables
        //Some dupe code, I could continue to handle input in PlayerMachine, but already alot there
        private InputController _input;
        AmmoType ammoType;


        public float coolDownTime = 5.0f;
        float coolDownLeft;


        public float damageAmpCap = 2.0f;
        public float speedAmpCap = 1.0f;
        float damageAmp;
        float speedAmp;

        //Only can charge one bullet at a time
        GameObject bulletCharging;
        public Transform bulletSpawnPoint;

#endregion
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


        public void Initialize()
        {

            currentState = PlayerAttackType.Ranged;
            ammoType = AmmoType.DefaultAmmo;


            foreach (var ammo in System.Enum.GetValues(typeof(AmmoType)))
            {
                GameObject prefab = Instantiate(Resources.Load("Prefab/PlayerAmmo/" + ammoType.ToString()) as GameObject);
                PoolManager.instance.AddPool(ammo.ToString(),prefab.GetComponent<PooledObject>(), 12);
            }
        }

        protected override void EarlyGlobalSuperUpdate()
        {
            base.EarlyGlobalSuperUpdate();           
        }

        protected override void LateGlobalSuperUpdate()
        {
            base.LateGlobalSuperUpdate();
            //Replace with controller input later, using unity input for testing

            if (GetComponent<Viral.ControlSystem.PlayerMachine>().IsStunned) { return; }


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
            if (GetComponent<Viral.ControlSystem.PlayerMachine>().IsStunned) { return; }

            if (UnityEngine.Input.GetKey(KeyCode.F))
            {
                coolDownLeft = coolDownTime;
                //ToDo: Animaion+ check for nearest enemy within collision of melee range
            }
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

            if (coolDownLeft <= 0) {


                if (Input.Current.AttackInput)
                {
                    if (GetComponent<Viral.ControlSystem.PlayerMachine>().IsStunned) { return; }

                    if (bulletCharging == null)
                    {
                        Debug.Log("Prefab/PlayerAmmo" + ammoType.ToString());
                        //bulletCharging = Instantiate(Resources.Load("Prefab/PlayerAmmo/" + ammoType.ToString()) as GameObject);
                        PooledObject ammoPrefab = PoolManager.instance.Acquire(ammoType.ToString());
                        bulletCharging = ammoPrefab.gameObject;

                        //Will create spawn point for this, ugly and bad to keep shoving this shit but fuck itt
                        bulletCharging.transform.position = bulletSpawnPoint.position;
                        Viral.StatSystem.OzStatCollection statCollection = GetComponent<Viral.StatSystem.OzStatCollection>();
                        bulletCharging.GetComponent<Ammo>().killedVirus += () => { ((Viral.StatSystem.StatVital)statCollection[Viral.StatSystem.StatType.KillCount]).Value += 1; GetComponent<PlayerMachine>().killCount += 1; };
                        bulletCharging.SetActive(true);
                        bulletCharging.transform.parent = transform;
                    }
                    else
                    {
                        damageAmp += Time.deltaTime;
                        speedAmp += Time.deltaTime * Time.deltaTime;
                        
                    }
                }
                if (!Input.Current.AttackInput)
                {
                    

                    if (bulletCharging != null)
                    {
                        if (coolDownLeft <= 0)
                            coolDownLeft = coolDownTime;

                        bulletCharging.transform.parent = null;
                        StartCoroutine(bulletCharging.GetComponent<Ammo>().Shoot(damageAmp, speedAmp, GetComponent<PlayerMachine>().facingRight ? 1 : -1));
                        bulletCharging = null;
                        damageAmp = 0;
                        speedAmp = 0;
                    }

                }
               

             
            }
            else if (coolDownLeft > 0)
            {

                coolDownLeft -= Time.deltaTime;
            }
        }

        void Ranged_ExitState()
        {
            bulletCharging = null;
            damageAmp = 0;
            speedAmp = 0;
        }

    }
}