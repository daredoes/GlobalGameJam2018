using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class PooledObject : MonoBehaviour {

    public delegate void Death();
    public event Death OnDeath;
    
    //Because cannot call event directly from inherited.
    protected void BackToPool()
    {
        if (OnDeath != null)
            OnDeath();
    }
}
    