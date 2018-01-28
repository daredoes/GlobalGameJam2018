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
        }

        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start () {

        pools = new Dictionary<string, Queue<PooledObject>>();
	}
	
    public void AddPool(string poolID, PooledObject obj, int poolSize)
    {
        if (!pools.ContainsKey(poolID))
        {
            pools[poolID] = new Queue<PooledObject>();

            for (int i = 0; i < poolSize; ++i)
            {
                PooledObject spawned = (Instantiate(obj.gameObject)).GetComponent<PooledObject>();
                pools[poolID].Enqueue(spawned);
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
            obj = Instantiate((Resources.Load("Prefabs/" + type) as GameObject)).GetComponent<PooledObject>();
            if (obj == null)
            {
                throw new System.Exception("Coudl not find PooledObject of that type");
            }
        }

        //It should keep closure on the type local as well? Maybe wrong will test
        obj.OnDeath += () => { pools[type].Enqueue(obj); };

        return obj;
    } 

	
}
