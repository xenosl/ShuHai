using System;
using System.Linq;
using ShuHai.Unity;
using ShuHai.Unity.UI.Widgets;
using UnityEngine.UI;

namespace ShuHai.Graphicode.Unity.UI
{
    public class UnitSelector : Panel
    {
        protected UnitSelector()
        {
            _searchInput = new Lazy<InputField>(FindSearchInput);
            _unitList = new Lazy<UnitList>(FindList);
        }

        #region Search

        public void FocusOnSearchInput()
        {
            SearchInput.Select();
            //SearchInput.ActivateInputField();
        }

        private InputField SearchInput => _searchInput.Value;

        private readonly Lazy<InputField> _searchInput;

        private InputField FindSearchInput() { return gameObject.FindComponentInChild<InputField>("Search"); }

        private void EnableSearchInput() { SearchInput.onValueChanged.AddListener(OnSearchTextChanged); }

        private void DisableSearchInput() { SearchInput.onValueChanged.RemoveListener(OnSearchTextChanged); }

        private void OnSearchTextChanged(string text)
        {
            foreach (var item in UnitList.Items)
            {
                item.Active = string.IsNullOrEmpty(text)
                    || item.UnitType.FullName.Contains(text, StringComparison.OrdinalIgnoreCase);
            }
        }

        #endregion Search

        #region Unit List

        public event Action<Type> SelectUnit;

        private UnitList UnitList => _unitList.Value;

        private readonly Lazy<UnitList> _unitList;

        private UnitList FindList() { return gameObject.FindComponentInChild<UnitList>("UnitList"); }

        private void PopulateUnitList()
        {
            var unitTypes = Assemblies.Instances
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(Unit)) && !t.IsAbstract)
                .OrderBy(t => t.Namespace);
            UnitList.CreateItems(unitTypes);
        }

        private void EnableUnitList() { UnitList.ItemClick += OnUnitListItemClick; }
        private void DisableUnitList() { UnitList.ItemClick -= OnUnitListItemClick; }

        private void OnUnitListItemClick(UnitListItem item) { SelectUnit?.Invoke(item.UnitType); }

        #endregion Unit List

        #region Unity Events

        protected override void OnEnable()
        {
            base.OnEnable();
            EnableSearchInput();
            EnableUnitList();
        }

        protected override void OnDisable()
        {
            DisableUnitList();
            DisableSearchInput();
            base.OnDisable();
        }

        protected override void Start()
        {
            base.Start();
            PopulateUnitList();
        }

        #endregion Unity Events
    }
}