using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace ShuHai.Unity
{
    public struct LogInfo
    {
        public string Log;
        public string StackTrack;
        public LogType LotType;
    }

    public static class Root
    {
        #region Life Cycle

        public static event Action FixedUpdate;
        public static event Action Update;
        public static event Action LateUpdate;

        public static bool ApplicationQuiting { get; internal set; }

        internal static void _Initialize()
        {
            InitializeLog();

            CreateComponents();
        }

        internal static void _Deinitialize()
        {
            DestroyComponents();

            DeinitializeLog();
        }

        internal static void _FixedUpdate() { FixedUpdate?.Invoke(); }

        internal static void _Update() { Update?.Invoke(); }

        internal static void _LateUpdate()
        {
            LateUpdate?.Invoke();
            LogLateUpdate();
        }

#if UNITY_EDITOR

        public static event Action EditorUpdate;

        internal static void EditorRootUpdate() { EditorUpdate?.Invoke(); }

#endif // UNITY_EDITOR

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            return RootBehaviour.Instance.StartCoroutine(routine);
        }

        public static void StopCoroutine(IEnumerator routine) { RootBehaviour.Instance.StopCoroutine(routine); }

        #endregion Life Cycle

        #region Components

        public static IReadOnlyList<MonoBehaviour> Components => _components;

        public static T GetComponent<T>()
            where T : MonoBehaviour
        {
            return (T)_componentsByType.GetValue(typeof(T));
        }

        public static MonoBehaviour GetComponent(Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<IRootComponent>(type, nameof(type));
            return _componentsByType.GetValue(type);
        }

        private static readonly List<MonoBehaviour> _components = new List<MonoBehaviour>();

        private static readonly Dictionary<Type, MonoBehaviour>
            _componentsByType = new Dictionary<Type, MonoBehaviour>();

        #region Initialization

        private static void CreateComponents()
        {
            var types = ShuHai.Assemblies.Instances
                .SelectMany(a => a.GetTypes())
                .Where(IsComponentType)
                .ToArray();

            _components.AddRange(types.Select(CreateComponent));
            _components.Sort(CompareComponent);

            foreach (var component in _components)
                _componentsByType.Add(component.GetType(), component);
        }

        private static MonoBehaviour CreateComponent(Type type)
        {
            return (MonoBehaviour)RootBehaviour.Instance.gameObject.AddComponent(type);
        }

        private static bool IsComponentType(Type type)
        {
            if (!typeof(IRootComponent).IsAssignableFrom(type))
                return false;
            if (!type.IsSubclassOf(typeof(Component)))
                return false;
            if (type.IsAbstract)
                return false;
            if (type.IsGenericType && type.ContainsGenericParameters)
                return false;
            return true;
        }

        private static int CompareComponent(MonoBehaviour l, MonoBehaviour r)
        {
            Type lt = l.GetType(), rt = r.GetType();
            var la = lt.GetCustomAttribute<RootComponentAttribute>();
            var ra = rt.GetCustomAttribute<RootComponentAttribute>();

            int lp = la?.Priority ?? 0, rp = ra?.Priority ?? 0;
            int c = -lp.CompareTo(rp);
            if (c != 0)
                return c;

            // Make sure the invocation order keeps the same when priority is the same.
            return string.Compare(lt.FullName, rt.FullName, StringComparison.Ordinal);
        }

        #endregion Initialization

        private static void DestroyComponents() { }

        #endregion Components

        #region Log

        /// <summary>
        ///     Log information of last unity's <see cref="LogType.Error" />, <see cref="LogType.Assert" /> or
        ///     <see cref="LogType.Exception" /> message.
        /// </summary>
        public static LogInfo? LastErrorLog { get; set; }

        /// <summary>
        ///     Similar to <see cref="LastErrorLog" /> but clear at end of each frame.
        /// </summary>
        public static LogInfo? LastErrorLogInFrame { get; set; }

        private static void InitializeLog() { Application.logMessageReceived += OnLogMessageReceived; }

        private static void DeinitializeLog() { Application.logMessageReceived -= OnLogMessageReceived; }

        private static void LogLateUpdate() { LastErrorLogInFrame = null; }

        private static void OnLogMessageReceived(string log, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Exception:
                case LogType.Error:
                case LogType.Assert:
                    LastErrorLog = new LogInfo
                    {
                        Log = log,
                        StackTrack = stackTrace,
                        LotType = type
                    };
                    LastErrorLogInFrame = LastErrorLog;
                    break;
                case LogType.Warning:
                    break;
                case LogType.Log:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        #endregion Log
    }
}
