using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRaB.Split.Util
{

    public class ObjectFollow : MonoBehaviour
    {
        [SerializeField]
        private GameObject toFollowObject;

        public bool X = false;
        public bool Y = false;
        public bool Z = false;

        [SerializeField]
        [Tooltip("Keep minimum distance less than maximum speed")]
        private float minDistance = 0.1f;
        [SerializeField]
        private float maxDistance = 10f;
        [Tooltip("Keep minimum speed low")]
        [SerializeField]
        private float minSpeed = 0.01f;
        [SerializeField]
        private bool enableMaxSpeed = false;
        [SerializeField]
        private float maxSpeed = 2f;
        [SerializeField]
        private bool isDeltaTime = true;

        private Transform myTransform;
        private Transform toFollow;

        private Vector3 myPosition;
        private Vector3 toFollowPosition;

        private Vector3 vForce;

        private bool isMoving = true;

        private void Awake()
        {
            SetInitialReferences();
        }

        private void FixedUpdate()
        {
            FollowObject(
                myTransform,
                toFollow,
                minDistance,
                maxDistance,
                minSpeed,
                enableMaxSpeed,
                maxSpeed,
                isDeltaTime,
                X,
                Y,
                Z
            );
        }

        public static bool FollowObject( Transform myTransform, Transform toFollow,
            float minDistance, float maxDistance, float minSpeed, bool enableMaxSpeed,
            float maxSpeed, bool isDeltaTime, bool X, bool Y, bool Z)
        {
            bool isMoving = true;

            Vector3 myPosition = myTransform.position;
            Vector3 toFollowPosition = toFollow.position;

            if (myPosition != toFollowPosition)
            {
                Vector3 vForce = new Vector3();
                if (X) {
                    if (myPosition.x > toFollowPosition.x + minDistance ||
                        myPosition.x < toFollowPosition.x - minDistance)
                    {
                        isMoving = false;
                        vForce.x = CalculateForceLerp(
                            myPosition.x,
                            toFollowPosition.x,
                            maxDistance,
                            minSpeed,
                            maxSpeed,
                            enableMaxSpeed,
                            isDeltaTime
                        );
                    }
                    else
                    {
                        isMoving = true;
                    }
                }
                if (Y) {
                    if (myPosition.y > toFollowPosition.y + minDistance ||
                        myPosition.y < toFollowPosition.y - minDistance)
                    {
                        vForce.y = CalculateForceLerp(
                            myPosition.y,
                            toFollowPosition.y,
                            maxDistance,
                            minSpeed,
                            maxSpeed,
                            enableMaxSpeed,
                            isDeltaTime
                        );
                    }
                    else
                    {
                        isMoving = true;
                    }
                }
                if (Z) {
                    if (myPosition.z > toFollowPosition.z + minDistance ||
                        myPosition.z < toFollowPosition.z - minDistance)
                    {
                        vForce.z = CalculateForceLerp(
                            myPosition.z,
                            toFollowPosition.z,
                            maxDistance,
                            minSpeed,
                            maxSpeed,
                            enableMaxSpeed,
                            isDeltaTime
                        );
                    }
                    else
                    {
                        isMoving = true;
                    }
                }
                myTransform.Translate(vForce);
            }
            return isMoving;
        }

        public static float CalculateForceLerp(float mPos, float tPos, float maxD,
            float minS, float maxS, bool isMaxS, bool isDTime)
        {
            float dist = mPos - tPos;
            float force = (1 / (maxD / Mathf.Abs(dist)) + minS) * (isDTime ? Time.deltaTime : 1f); // uses 1/(ratio of how near) + min speed (1/x curve, y limit = infinity -> min speed)
            if (isMaxS) if (force > maxS) force = maxS;
            if (mPos > tPos) force *= -1;
            return force;
        }

        private void TrackPlayerFloatArr()
        { // makes game incredibly slow, looking into why rn. Probably the use of overuse of conditional operator being called per frame
            myPosition = myTransform.position;
            toFollowPosition = toFollow.position;
            // matrix of own and player coordinates
            float[] myCoordinates = Vector3ToFloatXYZ(X, Y, Z, myPosition);
            float[] playerCoordinates = Vector3ToFloatXYZ(X, Y, Z, toFollowPosition);
            float[] forceArr = new float[3]; // 3 for X,Y,Z coordinates

            for (int i = 0; i < myCoordinates.Length; ++i)
            {
                Debug.Log("i: "+i+" | myCoordinates[i]: " + myCoordinates[i] + " | playerCoordinates[i]: " + playerCoordinates[i]);
                if (myCoordinates[i] != 0 || playerCoordinates[i] != 0)
                { // dont calculate if both == 0
                    if (myCoordinates[i] > playerCoordinates[i] + minDistance ||
                        myCoordinates[i] < playerCoordinates[i] - minDistance)
                    { // dont calculate if within minDistrance
                        float dist = playerCoordinates[i] - myCoordinates[i];
                        float force = (1 / (maxDistance / Mathf.Abs(dist)) + minSpeed) * Time.deltaTime; // uses 1/(ratio of how near) + min speed
                        if (enableMaxSpeed) if (force > maxSpeed) force = maxSpeed;
                        if (myCoordinates[i] > playerCoordinates[i]) force *= -1;
                        forceArr[i] = force;
                        Debug.Log("i: "+i+" | force: "+force);
                    }

                }
            }
            Debug.Log(FloatXYZToVector3(X, Y, Z, forceArr));
            myTransform.Translate(FloatXYZToVector3(X, Y, Z, forceArr));
        }

        private Vector3 Vector3ToVector3XYZ( bool X, bool Y, bool Z, Vector3 orgVector )
        { // start converting track players to use this (get rid of coordinates)
            return new Vector3(
                X ? orgVector.x : 0f,
                Y ? orgVector.y : 0f,
                Z ? orgVector.z : 0f
            );
        }

        private float[] Vector3ToFloatXYZ( bool X, bool Y, bool Z, Vector3 orgVector)
        {
            float[] arr = { // this is stupid wont work back to vector3 (would work with method in ObjectFollow)
                    X ? orgVector.x: Y ? orgVector.y: Z ? orgVector.z: 0f,
                    Y ? X ? orgVector.y : Z ? orgVector.z: 0f : X ? Z ? orgVector.z : 0f : 0f,
                    Z ? Y ? X ? orgVector.z: 0f : 0f : 0f
                };
            return arr;
        }

        private Vector3 FloatXYZToVector3( bool X, bool Y, bool Z, float[] arr )
        {
            return new Vector3(
                X ? arr[0] : 0f,
                Y ? X ? arr[1] : arr[0] : 0f,
                Z ? X ? Y ? arr[2] : arr[1] : Y ? arr[1] : arr[0] : 0f
            );
        }

        private void SetInitialReferences()
        {
            if (toFollowObject == null)
            {
                Debug.Log("Error: playerObject is null");
             }
            else
            {
                try
                {
                    toFollow = toFollowObject.GetComponent<Transform>();
                    toFollowPosition = toFollow.position;
                }
                catch
                {
                    Debug.Log("Error: Could not reference toFollowObject transform.");
                }
            }
            try { myTransform = GetComponent<Transform>(); }
            catch { Debug.Log("Error: Could not reference gameObject's transform."); }
        }


    }

}
