using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Viral.ControlSystem.AttackSystem
{
    public class Ammo : PooledObject
    {


        public float damage;
        public float speed;
        public Viral.ControlSystem.AttackSystem.DamageType dmgType;
        public delegate void Delegate();
        public event Delegate killedVirus;
       


        //Actually shoot off the Ammo
        public IEnumerator Shoot(float damageAmp, float speedAmp, int direction)
        {
            damage *= damageAmp;
            speed *= speedAmp;
            //Probably just add force to be done with it, could deal with patterns alter

            while (this.gameObject.activeInHierarchy)
            {
                transform.Translate(Vector3.right * speed * direction);
                yield return new WaitForEndOfFrame();
            }


        }


        void OnTriggerEnter2D(Collider2D other)
        {

            //GetComponent<PooledObject>().BackToPool();
            //SO this is called meaning did collide

            if (other.CompareTag("Virus"))
            {
               
                dmgType = DamageType.MELEE;
                //Call take damage on some script on the AI, not sure what but on something
                other.GetComponent<Viral.ControlSystem.AiMachine>().TakeDamage(damage, dmgType, other.transform.position - transform.position, null);

                if (other.GetComponent<Viral.StatSystem.OzStatCollection>()[Viral.StatSystem.StatType.Health].Value <= 0)
                {
                    Destroy(other);
                    if (killedVirus != null)
                        killedVirus();
                }

            }
            if (!other.CompareTag("Player"))
                BackToPool();

        }





    }



}
