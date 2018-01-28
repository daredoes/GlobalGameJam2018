using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour {


    public float damage;
    public float speed;


    //Actually shoot off the Ammo
	public IEnumerator Shoot(float damageAmp, float speedAmp, int direction)
    {
        damage *= damageAmp;
        speed *= speedAmp;


        //Probably just add force to be done with it, could deal with patterns alter

        while (this.gameObject.activeInHierarchy)
        {
            transform.Translate(Vector3.right * speed * direction);
            Debug.Log("hello");
            yield return new WaitForEndOfFrame();
        }
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
