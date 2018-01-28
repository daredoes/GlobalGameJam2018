using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Viral.StatSystem;

namespace Viral.EntitySystem {
    public class Entity : MonoBehaviour
    {
        StatCollection _statCollection;
        StatCollection statCollection {
            get {
                if(!_statCollection) {
                    _statCollection = GetComponent<StatCollection>();
                }
                return _statCollection;
            }
        }
    }   
}