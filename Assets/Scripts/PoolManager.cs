﻿using System.Collections;
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
                PooledObject spawned = ((GameObject)Instaniate(obj.GameObject)).GetComponent<PooledObject>();
                pools[poolID].Enqueue(spawned);
            }

        } 
    }

    public void Acquire(string type)
    {
        PooledObject obj = pools[type].Dequeue();

        if (obj == null)
        {
            obj = Instantiate((Resources.Load("Prefabs/" + type) as GameObject)).GetComponent<PooledObject>();
        }

        //It should keep closure on the type local as well? Maybe wrong will test
        obj.OnDeath += () => { pools[type].Enqueue(obj); };

        return obj;
    } 

	
}
