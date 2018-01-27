using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Viral
{
    public class GUIManager : MonoBehaviour
    {

        static protected GUIManager _instance;
        static public GUIManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != null)
            {
                Debug.Log(this.GetType().Name + " is already in play. Deleting new!", gameObject);
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
