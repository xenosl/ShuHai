using System;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.Demo
{
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        public static Transform Transform { get; private set; }

        public static GameObjectGraph Graph { get; private set; }

        public static Vector3 GetPositionAlongSightLine(float distance)
        {
            return Transform.position + Transform.forward * distance;
        }

        private void Awake()
        {
            if (Instance)
                throw new Exception("Duplicate instance of singleton.");

            Instance = this;
            Transform = transform;
            Graph = GetComponent<GameObjectGraph>();
        }

        private void OnDestroy()
        {
            Debug.Assert(Instance == this);

            Transform = null;
            Instance = null;
        }
    }
}
