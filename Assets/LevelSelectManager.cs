using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectManager : MonoBehaviour {

    enum PriorityLevel
    {
        NULL   = -1,
        LOW    =  0,
        MEDIUM =  1,
        HIGH   =  2
    }

    struct LevelInfo
    {
        public string name;
        public PriorityLevel priority;
        public int daysLeft;
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
