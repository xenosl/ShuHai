using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity
{
    internal sealed class RootBehaviour : MonoBehaviour
    {
        private void Awake() { Instance = this; }

        private void OnDestroy() { Instance = null; }

        private void Start()
        {
            if (EnsureSingleton())
                Root.Initialize();
        }

        private void FixedUpdate()
        {
            if (EnsureSingleton())
                Root.FixedUpdate();
        }

        private void Update()
        {
            if (EnsureSingleton())
                Root.Update();
        }

        private void LateUpdate()
        {
            if (EnsureSingleton())
                Root.LateUpdate();
        }

        private void OnApplicationQuit()
        {
            if (EnsureSingleton())
                Root.Deinitialize();

            DestroyImmediate(gameObject);
        }

        private bool EnsureSingleton()
        {
            if (Instance == this)
                return true;
            DestroyInstance(this);
            return false;
        }

        private RootBehaviour() { _instances.Add(this); }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            Instance = SceneManagerEx.FindComponent<RootBehaviour>();
            if (!Instance)
            {
                var gameObject = new GameObject(typeof(RootBehaviour).FullName) { hideFlags = HideFlags.DontSave };
                DontDestroyOnLoad(gameObject);
                gameObject.AddComponent<RootBehaviour>();
            }
            MakeSingleton(Instance);
        }

        #region Instances

        public static RootBehaviour Instance { get; private set; }

        private static readonly HashSet<RootBehaviour> _instances = new HashSet<RootBehaviour>();

        private static void MakeSingleton(RootBehaviour instance)
        {
            UnityEnsure.Argument.NotNull(instance, nameof(instance));

            foreach (var inst in _instances.Where(inst => inst != instance).ToArray())
                DestroyInstance(inst);
        }

        private static void DestroyInstance(RootBehaviour inst)
        {
            UnityObjectUtil.Destroy(inst.gameObject);
            _instances.Remove(inst);
        }

        #endregion Instances
    }
}
