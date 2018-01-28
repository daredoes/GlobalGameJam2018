using UnityEngine;

namespace Util
{
    public class DDOL : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
