using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {


    public float damage;
    public float speed;


    //Actually shoot off the Ammo
	public void Shoot(float damageAmp, float speedAmp)
    {
        damage *= damageAmp;
        speed *= speedAmp;


        //Probably just add force to be done with it, could deal with patterns alter
        GetComponent<Rigidbody2D>().AddForce(transform.forward * speed);
    }

    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Virus"))
        {
          
            //Haven't merged in object pooling so commented out for now
          //  GetComponent<PooledObject>().BackToPool();
            

            //Need to hurt other viruses by this bullet's damage.
        }

    }



}
