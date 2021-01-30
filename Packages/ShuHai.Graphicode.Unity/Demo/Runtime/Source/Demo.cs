using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Graphicode.Unity.UI;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.Demo
{
    using StateMachine = StateMachine<DemoMode>;

    public enum DemoMode
    {
        Inactive,
        Play,
        Edit
    }

    public class Demo : MonoBehaviour
    {
        #region Modes

        public static DemoMode Mode { get => StateMachine.State; set => StateMachine.State = value; }

        private static readonly StateMachine StateMachine = new StateMachine(DemoMode.Inactive);

        private static void InitializeStates()
        {
            StateMachine.RegisterChangeEvents(DemoMode.Play, OnPlayModeEntered, OnPlayModeExiting);
            StateMachine.RegisterChangeEvents(DemoMode.Edit, OnEditModeEntered, OnEditModeExiting);
        }

        private static void DeinitializeStates()
        {
            StateMachine.UnregisterChangeEvents(DemoMode.Edit, OnEditModeEntered, OnEditModeExiting);
            StateMachine.UnregisterChangeEvents(DemoMode.Play, OnPlayModeEntered, OnPlayModeExiting);
        }

        private static void StartMode() { Mode = DemoMode.Play; }

        private static void StopMode() { Mode = DemoMode.Inactive; }

        private static void OnPlayModeEntered(StateMachine.Transition t, DemoMode src, DemoMode dst, object[] args)
        {
            EnterMode(DemoMode.Play);
            Descriptions.ActiveRectMode = DemoMode.Play;
        }

        private static void OnPlayModeExiting(StateMachine.Transition t, DemoMode src, DemoMode dst, object[] args)
        {
            ExitMode(DemoMode.Play);
        }

        private static void OnEditModeEntered(StateMachine.Transition t, DemoMode src, DemoMode dst, object[] args)
        {
            Descriptions.ActiveRectMode = DemoMode.Edit;
            EnterMode(DemoMode.Edit);
        }

        private static void OnEditModeExiting(StateMachine.Transition t, DemoMode src, DemoMode dst, object[] args)
        {
            ExitMode(DemoMode.Edit);
        }

        #region Handlers

        public static IReadOnlyList<ModeHandler> ModeHandlers;

        public static ModeHandler CurrentModeHandler => GetModeHandler(Mode);

        private static void InitializeModeHandles()
        {
            ModeHandlers = new[] { (ModeHandler)null, Instance._playModeHandler, Instance._editModeHandler };
            VerifyHandlers();

            foreach (var handler in ModeHandlers)
            {
                if (handler)
                    handler.Initialize();
            }
        }

        private static void DeinitializeModeHandlers()
        {
            foreach (var handler in ModeHandlers.Reverse())
            {
                if (handler)
                    handler.Deinitialize();
            }
        }

        private static void EnterMode(DemoMode mode) { GetModeHandler(mode).enabled = true; }
        private static void ExitMode(DemoMode mode) { GetModeHandler(mode).enabled = false; }

        private static ModeHandler GetModeHandler(DemoMode mode) { return ModeHandlers[(int)mode]; }

        private static void VerifyHandlers()
        {
            for (int i = 0; i < ModeHandlers.Count; ++i)
            {
                var mode = EnumTraits<DemoMode>.Values[i];
                if (mode == DemoMode.Inactive)
                    continue;

                var handler = ModeHandlers[i];
                if (!handler)
                    throw new MissingReferenceException($"Missing handler reference for mode '{mode}'.");
                if (mode != handler.Mode)
                    throw new InvalidReferenceException($"Invalid handler for mode '{mode}'.");
            }
        }

#pragma warning disable 0649
        [SerializeField] private PlayModeHandler _playModeHandler;
        [SerializeField] private EditModeHandler _editModeHandler;
#pragma warning restore 0649

        #endregion Handlers

        #endregion Modes

        #region UI

        public static Canvas ScreenCanvas => Instance._screenCanvas;
        public static Canvas WorldCanvas => Instance._worldCanvas;

        public static Descriptions Descriptions => Instance._descriptions;

        public static GraphPanel LevelGraphPanel => Instance._levelGraphPanel;

#pragma warning disable 0649
        [SerializeField] private Canvas _screenCanvas;
        [SerializeField] private Canvas _worldCanvas;
        [SerializeField] private Descriptions _descriptions;
        [SerializeField] private GraphPanel _levelGraphPanel;
#pragma warning restore 0649

        private static void InitializeUI() { LevelGraphPanel.Graph = new Graph(); }

        private static void DeinitializeUI() { LevelGraphPanel.Graph = null; }

        #endregion UI

        #region Unity Events

        private void Awake()
        {
            if (Instance)
                throw new InvalidOperationException("Only one instance of Demo is allowed.");
            Instance = this;

            InitializeUI();
            InitializeModeHandles();
            InitializeStates();
        }

        private void OnDestroy()
        {
            StopMode();

            DeinitializeStates();
            DeinitializeModeHandlers();
            DeinitializeUI();

            Instance = null;
        }

        private void Start() { StartMode(); }

        private void OnApplicationQuit() { DestroyImmediate(gameObject); }

        #endregion Unity Events

        public static Demo Instance { get; private set; }
    }
}
