using System;
using System.Collections.Generic;
using System.Linq;
using ShuHai.Unity.UI;
using UnityEngine;

namespace ShuHai.Graphicode.Unity.UI
{
    public class UnitList : Widget
    {
        #region Items

        public UnitListItem ItemTemplate;

        public event Action<UnitListItem> ItemClick;

        public IReadOnlyCollection<UnitListItem> Items => _items.Values;

        public UnitListItem CreateItem(Type unitType)
        {
            EnsureUnitTypeArgument(unitType);
            if (!ItemTemplate)
                throw new InvalidOperationException("Item template is required to create an item.");

            var item = Instantiate(ItemTemplate, ItemContainer);
            SetupItem(item, unitType);
            AddItem(item);
            return item;
        }

        public IEnumerable<UnitListItem> CreateItems(IEnumerable<Type> unitTypes)
        {
            // Note that the ToArray is necessary to make sure items are created here but not delayed.
            return unitTypes.Select(CreateItem).ToArray();
        }

        public bool DestroyItem(Type unitType)
        {
            EnsureUnitTypeArgument(unitType);

            if (!_items.TryGetValue(unitType, out var item))
                return false;

            _items.Remove(unitType);
            if (item)
            {
                TeardownItem(item);
                Destroy(item.gameObject);
            }
            return true;
        }

        public void ClearItems()
        {
            foreach (var unitType in _items.Keys.ToArray())
                DestroyItem(unitType);
        }

        /// <summary>
        ///     Find existing items in children and add them to current list, destroy redundant children.
        /// </summary>
        public void ValidateItems()
        {
            var invalidChildren = new List<Transform>();
            foreach (var child in ItemContainer.Cast<Transform>().ToArray())
            {
                var item = child.GetComponent<UnitListItem>();
                if (item)
                {
                    var unitType = item.UnitType;
                    if (unitType != null)
                    {
                        if (_items.TryGetValue(unitType, out var existedItem))
                            SetupItem(existedItem, unitType);
                        else
                            AddItem(item);
                    }
                    else
                    {
                        invalidChildren.Add(child);
                    }
                }
                else
                {
                    invalidChildren.Add(child);
                }
            }

            foreach (var child in invalidChildren)
                Destroy(child.gameObject);
        }

        public UnitListItem GetItem(Type unitType) { return _items.GetValue(unitType); }

        public UnitListItem GetItem(int index)
        {
            Ensure.Argument.ValidIndex(index, _items.Count, nameof(index));
            Debug.Assert(ItemContainer.childCount == _items.Count);
            return ItemContainer.GetChild(index).GetComponent<UnitListItem>();
        }

        private readonly Dictionary<Type, UnitListItem> _items = new Dictionary<Type, UnitListItem>();

        private float _lastItemContainerWidth = float.NaN;

        private void ItemsUpdate()
        {
            if (ItemContainer.rect.width != _lastItemContainerWidth)
                Items.ForEach(UpdateItemWidth);
            _lastItemContainerWidth = ItemContainer.rect.width;
        }

        private void AddItem(UnitListItem item) { _items.Add(item.UnitType, item); }

        private void UpdateItemWidth(UnitListItem item)
        {
            var size = item.RectTransform.sizeDelta;
            size.x = ItemContainer.rect.width;
            item.RectTransform.sizeDelta = size;
        }

        private void SetupItem(UnitListItem item, Type unitType)
        {
            item.UnitType = unitType;
            item.Owner = this;

            item.ButtonClick += OnItemClick;
        }

        private void TeardownItem(UnitListItem item)
        {
            item.ButtonClick -= OnItemClick;

            item.Owner = null;
            item.UnitType = null;
        }

        private void OnItemClick(UnitListItem item) { ItemClick?.Invoke(item); }

        private static void EnsureUnitTypeArgument(Type unitType)
        {
            Ensure.Argument.NotNull(unitType, nameof(unitType));
            Ensure.Argument.Is<Unit>(unitType, nameof(unitType));
        }

        #region Container Transform

        public RectTransform ItemContainer
        {
            get => _itemContainer;
            set
            {
                if (value == _itemContainer)
                    return;
                if (!value.IsChildOf(RectTransform))
                    throw new ArgumentException("Child of current list is required.", nameof(value));

                _itemContainer = value;
            }
        }

        [SerializeField] private RectTransform _itemContainer;

        private void ValidateItemContainer()
        {
            if (_itemContainer && !_itemContainer.IsChildOf(RectTransform))
                _itemContainer = null;
        }

        #endregion Container Transform

        #endregion Items

        #region Unity Events

        private void Update() { ItemsUpdate(); }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ValidateItemContainer();
        }
#endif

        #endregion Unity Events
    }
}