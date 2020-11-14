using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShuHai.Graphicode.Generator;
using ShuHai.Graphicode.Unity.Editor.UI;
using ShuHai.Unity.Editor;
using UnityEditor;
using UnityEngine.UIElements;

namespace ShuHai.Graphicode.Unity.Editor
{
    public class CodeGeneratorWindow : EditorWindow
    {
        public const string Title = "Code Generator for CodeGraph";

        [MenuItem(MenuInfo.Path + Title)]
        public static CodeGeneratorWindow Open()
        {
            var w = Get();
            w.Show();
            return w;
        }

        public static CodeGeneratorWindow Get(string title = Title) { return GetWindow<CodeGeneratorWindow>(title); }

        private void Initialize()
        {
            var root = rootVisualElement;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/ShuHai/ShuHai.Graphicode.Unity.Editor/CodeGeneratorWindow.uxml");
            root.Add(visualTree.CloneTree());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Assets/ShuHai/ShuHai.Graphicode.Unity.Editor/CodeGeneratorWindow.uss");
            root.styleSheets.Add(styleSheet);

            InitializeAssemblyList();
            InitializeTypeList();
            InitializeMethodList();
            InitializeGenerate();
        }

        #region Assembly List

        private ListView<Assembly> _assemblyList;

        private void InitializeAssemblyList()
        {
            _assemblyList = new ListView<Assembly>(rootVisualElement.Q<VisualElement>("AssemblyList"))
            {
                Title = "Assemblies",
                SelectionType = SelectionType.Multiple,
                MakeItem = () => new Label("Assembly"),
                BindItem = (elem, item) => { ((Label)elem).text = item.GetName().Name; }
            };
            ResetAssemblyList();

            _assemblyList.FilterTextChanged += OnAssemblyFilterTextChanged;
            _assemblyList.SelectionChanged += OnAssemblySelectionChanged;
        }

        private void ResetAssemblyList()
        {
            _assemblyList.Clear();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => FilterAssembly(a, _assemblyList.FilterText));
            _assemblyList.AddRange(assemblies);
        }

        private void OnAssemblyFilterTextChanged(string oldText) { ResetAssemblyList(); }

        private void OnAssemblySelectionChanged(IReadOnlyList<Assembly> oldSelections) { ResetTypeList(); }

        private static bool FilterAssembly(Assembly assembly, string filterText)
        {
            if (assembly.IsDynamic)
                return false;

            return filterText == null
                || assembly.GetName().Name.Contains(filterText, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Assembly List

        #region Type List

        private ListView<Type> _typeList;

        private void InitializeTypeList()
        {
            _typeList = new ListView<Type>(rootVisualElement.Q<VisualElement>("TypeList"))
            {
                Title = "Types",
                SelectionType = SelectionType.Multiple,
                MakeItem = () => new Label("Type"),
                BindItem = (elem, item) => { ((Label)elem).text = item.FullName; }
            };
            ResetTypeList();

            _typeList.FilterTextChanged += OnTypeFilterTextChanged;
            _typeList.SelectionChanged += OnTypeSelectionChanged;
        }

        private void ResetTypeList()
        {
            var assemblies = _assemblyList.Selections;

            _typeList.Title = $"Types of {string.Join(",", assemblies.Select(a => a.GetName().Name))}";

            string filterText = _typeList.FilterText;
            _typeList.Clear();
            var types = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(IsTypeSupported)
                .Where(t => filterText == null || t.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase));
            _typeList.AddRange(types);
        }

        private void OnTypeFilterTextChanged(string oldText) { ResetTypeList(); }

        private void OnTypeSelectionChanged(IReadOnlyList<Type> oldSelections) { ResetMethodList(); }

        private static bool IsTypeSupported(Type type)
        {
            return type.IsPublic
                && !typeof(Exception).IsAssignableFrom(type)
                && (!type.IsGenericType || type.IsClosedConstructedType());
        }

        #endregion Type List

        #region Method List

        private ListView<MethodInfo> _methodList;

        private void InitializeMethodList()
        {
            _methodList = new ListView<MethodInfo>(rootVisualElement.Q<VisualElement>("MethodList"))
            {
                Title = "Methods",
                SelectionType = SelectionType.Multiple,
                MakeItem = () => new Label("Method"),
                BindItem = (elem, item) => { ((Label)elem).text = item.MakeName(); }
            };

            _methodList.FilterTextChanged += OnMethodFilterTextChanged;
        }

        private void ResetMethodList()
        {
            var types = _typeList.Selections;

            _methodList.Title = $"Methods of {string.Join(",", types.Select(t => t.Name))}";

            _methodList.Clear();
            _methodList.AddRange(types.SelectMany(GetMethods).Where(FilterMethod));
        }

        private void OnMethodFilterTextChanged(string oldText) { ResetMethodList(); }

        private bool FilterMethod(MethodInfo method)
        {
            if (!UnitCodeGenerator.IsSupported(method))
                return false;

            var filterText = _methodList.FilterText;
            return filterText == null || method.Name.Contains(filterText, StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsMethodSupported(MethodInfo method) { return UnitCodeGenerator.IsSupported(method); }

        private static MethodInfo[] GetMethods(Type type)
        {
            return type.GetMethods(BindingFlags.DeclaredOnly
                | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
        }

        #endregion Method List

        #region Generate

        private Button _generateButton;

        private void InitializeGenerate()
        {
            _generateButton = rootVisualElement.Q<Button>("Generate");
            _generateButton.clickable.clicked += OnGenerateButtonClick;
        }

        private void OnGenerateButtonClick()
        {
            var selectedMethods = _methodList.Selections;
            if (selectedMethods.Count == 0)
                return;

            foreach (var method in selectedMethods)
                CodeGenerator.Generate(method);
            AssetDatabase.Refresh();
        }

        #endregion Generate

        #region Unity Events

        private void OnEnable() { Initialize(); }

        #endregion Unity Events
    }
}