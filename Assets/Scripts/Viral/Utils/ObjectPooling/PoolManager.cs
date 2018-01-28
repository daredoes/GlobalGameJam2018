using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {


    Dictionary<string, Queue<PooledObject>> pools;

    public static PoolManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
            return;
        }

        pools = new Dictionary<string, Queue<PooledObject>>();
        DontDestroyOnLoad(this);
    }


    public void AddPool(string poolID, PooledObject obj, int poolSize)
    {
        if (instance.pools == null)
        {
            instance.pools = new Dictionary<string, Queue<PooledObject>>();
        }
        if (!instance.pools.ContainsKey(poolID))
        {
            instance.pools[poolID] = new Queue<PooledObject>();

            for (int i = 0; i < poolSize; ++i)
            {
                PooledObject spawned = (Instantiate(obj.gameObject)).GetComponent<PooledObject>();
                spawned.gameObject.SetActive(false);
                spawned.transform.parent = transform;
                instance.pools[poolID].Enqueue(spawned);
                Debug.Log("ghgh");
            }

        }
        else
        {
            throw new System.Exception("That poolID is already taken");
        }
    }

    public PooledObject Acquire(string type)
    {
        PooledObject obj = pools[type].Dequeue();

        if (obj == null)
        {
            Debug.Log("nuttin");
            obj = Instantiate((Resources.Load("Prefabs/" + type) as GameObject)).GetComponent<PooledObject>();
            if (obj == null)
            {
                throw new System.Exception("Coudl not find PooledObject of that type");
            }
        }

        //It should keep closure on the type local as well? Maybe wrong will test
        obj.OnDeath += () => { obj.gameObject.SetActive(false); instance.pools[type].Enqueue(obj); obj.transform.parent = transform; };

        return obj;
    } 

	
}
