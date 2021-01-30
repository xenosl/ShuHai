using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ShuHai.Unity
{
    [DefaultExecutionOrder(1000)]
    [AddComponentMenu(ShuHai.AssemblyInfo.RootNamespace + "/Root")]
    internal sealed class RootBehaviour : MonoBehaviour
    {
        #region Instances

        private RootBehaviour() { _instances.Add(this); }

        private bool EnsureSingleton()
        {
            if (Instance == this)
                return true;
            DestroyInstance(this);
            return false;
        }

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

        #region Unity Events

        private void Awake()
        {
            MakeSingleton(this);
            Instance = this;
        }

        private void OnDestroy() { Instance = null; }

        private void Start()
        {
            if (EnsureSingleton())
                Root._Initialize();
        }

        private void FixedUpdate()
        {
            if (EnsureSingleton())
                Root._FixedUpdate();
        }

        private void Update()
        {
            if (EnsureSingleton())
                Root._Update();
        }

        private void LateUpdate()
        {
            if (EnsureSingleton())
                Root._LateUpdate();
        }

        private void OnApplicationQuit()
        {
            Root.ApplicationQuiting = true;

            if (EnsureSingleton())
                Root._Deinitialize();

            DestroyImmediate(gameObject);
        }

        #endregion Unity Events

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void Initialize()
        {
            var obj = new GameObject(typeof(Root).FullName);
            DontDestroyOnLoad(obj);

            obj.AddComponent<RootBehaviour>();
        }
    }
}
