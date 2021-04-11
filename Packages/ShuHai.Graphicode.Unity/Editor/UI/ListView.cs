using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace ShuHai.Graphicode.Unity.Editor.UI
{
    /// <summary>
    ///     Generic version of <see cref="ListView" /> with extra optional elements.
    /// </summary>
    /// <typeparam name="T">Item type of the list.</typeparam>
    public class ListView<T> : IList<T>, IReadOnlyList<T>
    {
        #region UI

        public ListView(VisualElement root)
        {
            Ensure.Argument.NotNull(root, nameof(root));

            _root = root;

            TitleLabel = root.Q<Label>(TitleLabelName);

            FilterTextField = root.Q<TextField>(FilterTextFieldName);
            InitializeFilter();

            _itemListView = root.Q<ListView>(ItemListViewName);
            if (_itemListView == null)
                throw new ArgumentException($"A ListView named '{ItemListViewName}' is required.", nameof(root));
            InitializeItemListView();
        }

        private readonly VisualElement _root;

        #region Title

        /// <summary>
        ///     Shortcut for <see cref="Label.text" /> property of <see cref="Title" />.
        /// </summary>
        public string Title
        {
            get => TitleLabel != null ? TitleLabel.text : string.Empty;
            set
            {
                if (TitleLabel != null)
                    TitleLabel.text = value;
            }
        }

        public readonly Label TitleLabel;

        private const string TitleLabelName = "Title";

        #endregion Title

        #region Filter

        /// <summary>
        ///     Occurs after the string of the filter text field has changed.
        ///     Note that the parameter represents the string before change.
        /// </summary>
        public event Action<string> FilterTextChanged;

        /// <summary>
        ///     Shortcut for <see cref="TextField.text" /> property of <see cref="FilterTextField" />.
        /// </summary>
        public string FilterText => FilterTextField != null ? FilterTextField.text : string.Empty;

        public readonly TextField FilterTextField;

        private void InitializeFilter() { FilterTextField.RegisterValueChangedCallback(OnFilterTextChanged); }

        private void OnFilterTextChanged(ChangeEvent<string> evt) { FilterTextChanged?.Invoke(evt.previousValue); }

        private const string FilterTextFieldName = "Filter";

        #endregion Filter

        #region Item ListView

        public SelectionType SelectionType
        {
            get => _itemListView.selectionType;
            set => _itemListView.selectionType = value;
        }

        public int SelectedIndex
        {
            get => _itemListView.selectedIndex;
            set => _itemListView.selectedIndex = value;
        }

        public T SelectedItem => (T)_itemListView.selectedItem;

        /// <summary>
        ///     see <see cref="ListView.makeItem" />.
        /// </summary>
        public Func<VisualElement> MakeItem
        {
            get => _itemListView.makeItem;
            set => _itemListView.makeItem = value;
        }

        /// <summary>
        ///     Generic version of <see cref="ListView.bindItem" />.
        /// </summary>
        public Action<VisualElement, T> BindItem;

        private readonly ListView _itemListView;

        private void InitializeItemListView()
        {
            _itemListView.itemsSource = _items;
            _itemListView.bindItem = ItemListViewBindItem;
            _itemListView.onSelectionChange += OnSelectionChanged;
        }

        private void RefreshItemListView() { _root.schedule.Execute(() => _itemListView.Refresh()); }

        private void ItemListViewBindItem(VisualElement element, int index) { BindItem?.Invoke(element, this[index]); }

        private const string ItemListViewName = "List";

        #endregion Item ListView

        #region Selections

        /// <summary>
        ///     <para>Occurs when item selection changed.</para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Note that the parameter indicates the old selection. To obtain the newly selected items, consider
        ///         the property <see cref="Selections" />.
        ///     </para>
        /// </remarks>
        public event Action<IReadOnlyList<T>> SelectionChanged;

        public IReadOnlyList<T> Selections { get; private set; } = Array.Empty<T>();

        private void OnSelectionChanged(IEnumerable<object> value)
        {
            var old = Selections;
            Selections = value.Cast<T>().ToArray();
            SelectionChanged?.Invoke(old);
        }

        #endregion Selections

        #endregion UI

        #region Items

        public T this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public int Count => _items.Count;

        public void Add(T item)
        {
            _items.Add(item);
            RefreshItemListView();
        }

        public void AddRange(IEnumerable<T> items)
        {
            _items.AddRange(items);
            RefreshItemListView();
        }

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
            RefreshItemListView();
        }

        public bool Remove(T item)
        {
            bool removed = _items.Remove(item);
            if (removed)
                RefreshItemListView();
            return removed;
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            RefreshItemListView();
        }

        public void Clear()
        {
            _items.Clear();
            RefreshItemListView();
        }

        public int IndexOf(T item) { return _items.IndexOf(item); }

        public bool Contains(T item) { return _items.Contains(item); }

        public IEnumerator<T> GetEnumerator() { return _items.GetEnumerator(); }

        private readonly List<T> _items = new List<T>();

        #region Explicite Implementations

        bool ICollection<T>.IsReadOnly => false;
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) { _items.CopyTo(array, arrayIndex); }
        IEnumerator IEnumerable.GetEnumerator() { return ((IEnumerable)_items).GetEnumerator(); }

        #endregion Explicite Implementations

        #endregion Items
    }
}
