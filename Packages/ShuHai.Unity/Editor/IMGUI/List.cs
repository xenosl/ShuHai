using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ShuHai.Unity.Editor.IMGUI
{
    public class List<T> : IControl, IList<T>, IReadOnlyList<T>
        where T : IControl
    {
        public List() { }

        public List(IEnumerable<T> items) { Add(items); }

        #region Items

        public T this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }

        public int Count => items.Count;

        public void Add(T item)
        {
            Ensure.Argument.NotNull(item, nameof(item));
            AddImpl(item);
        }

        public void Add(IEnumerable<T> items)
        {
            Ensure.Argument.NotNull(items, nameof(items));

            foreach (var item in items)
                AddImpl(item);
        }

        private void AddImpl(T item)
        {
            items.Add(item);
            if (Count == 1)
                OnFirstItemAdded();
        }

        public bool Remove(T item)
        {
            Ensure.Argument.NotNull(item, nameof(item));

            int index = IndexOf(item);
            if (index < 0)
                return false;
            RemoveAtImpl(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            Ensure.Argument.ValidIndex(index, Count, nameof(index));
            RemoveAtImpl(index);
        }

        private void RemoveAtImpl(int index)
        {
            items.RemoveAt(index);
            if (Count == 0)
                OnLastItemRemoved();
        }

        public void Insert(int index, T item)
        {
            items.Insert(index, item);
            if (Count == 1)
                OnFirstItemAdded();
        }

        public void Clear()
        {
            items.Clear();
            OnLastItemRemoved();
        }

        public int IndexOf(T item) { return items.IndexOf(item); }

        public bool Contains(T item) { return items.Contains(item); }

        public IEnumerator<T> GetEnumerator() { return ((IEnumerable<T>)items).GetEnumerator(); }

        private readonly System.Collections.Generic.List<T> items = new System.Collections.Generic.List<T>();

        private void OnFirstItemAdded() { pageIndex = 0; }

        private void OnLastItemRemoved() { pageIndex = Index.Invalid; }

        bool ICollection<T>.IsReadOnly => false;
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) { items.CopyTo(array, arrayIndex); }

        #endregion Items

        #region Pages

        /// <summary>
        ///     Number of total pages.
        /// </summary>
        public int PageCount => Mathf.CeilToInt(Count / (float)ItemCountPerPage);

        /// <summary>
        ///     Index of currently displaying page.
        /// </summary>
        /// <remarks>
        ///     Note that value is <see cref="Index.Invalid" /> if the list is empty.
        /// </remarks>
        public int PageIndex
        {
            get => pageIndex;
            set => pageIndex = Count > 0 ? Mathf.Clamp(value, 0, PageCount - 1) : Index.Invalid;
        }

        /// <summary>
        ///     First item index of current page.
        /// </summary>
        /// <remarks>
        ///     Note that value is <see cref="Index.Invalid" /> if the list is empty.
        /// </remarks>
        public int FirstItemIndex => Count > 0 ? pageIndex * itemCountPerPage : Index.Invalid;

        /// <summary>
        ///     Last item index of current page.
        /// </summary>
        /// <remarks>
        ///     Note that value is <see cref="Index.Invalid" /> if the list is empty.
        /// </remarks>
        public int LastItemIndex
        {
            get
            {
                int pageCount = PageCount;
                switch (pageCount)
                {
                    case 0:
                        return Index.Invalid;
                    case 1:
                        return Count - 1;
                    default:
                        Debug.Assert(pageCount > 0);
                        return pageIndex == PageCount - 1 // Is last page?
                            ? pageIndex * itemCountPerPage + Count % itemCountPerPage - 1
                            : (pageIndex + 1) * itemCountPerPage - 1;
                }
            }
        }

        private int pageIndex = Index.Invalid;

        #endregion Pages

        #region Item&Page Relation

        public const int MinItemCountPerPage = 2;
        public const int MaxItemCountPerPage = 100;
        public const int DefaultItemCountPerPage = 10;

        /// <summary>
        ///     Number of items allowed in one page.
        /// </summary>
        public int ItemCountPerPage
        {
            get => itemCountPerPage;
            set
            {
                if (value == itemCountPerPage)
                    return;

                value = Mathf.Clamp(value, MinItemCountPerPage, MaxItemCountPerPage);
                itemCountPerPage = value;
            }
        }

        private int itemCountPerPage = DefaultItemCountPerPage;

        #endregion Item&Page Relation

        #region GUI

        public void OnGUI()
        {
            ItemsGUI();
            PagesGUI();
        }

        private void ItemsGUI()
        {
            if (Count == 0)
                return;

            for (int i = FirstItemIndex; i <= LastItemIndex; ++i)
            {
                var item = items[i];
                using (new EditorGUILayout.HorizontalScope())
                    item.OnGUI();
            }
        }

        private void PagesGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.Space();

                bool hasPrevPage = Index.IsValid(pageIndex - 1, PageCount);
                if (PageButtonGUI(hasPrevPage, "<"))
                    PageIndex--;

                PageNoGUI();

                bool hasNextPage = Index.IsValid(pageIndex + 1, PageCount);
                if (PageButtonGUI(hasNextPage, ">"))
                    PageIndex++;
            }
        }

        private void PageNoGUI()
        {
            bool hasItem = Count > 0;
            if (hasItem)
            {
                int pageNo = pageIndex + 1;
                pageNo = EditorGUILayout.DelayedIntField(pageNo);
                PageIndex = pageNo - 1;
            }
            else
            {
                GUI.enabled = false;
                EditorGUILayout.TextArea("-");
                GUI.enabled = true;
            }
        }

        private static readonly GUILayoutOption[] pageButtonLayoutOptions = { GUILayout.Width(24) };

        private static bool PageButtonGUI(bool enabled, string text)
        {
            GUI.enabled = enabled;
            bool click = GUILayout.Button(text, EditorStyles.miniButton, pageButtonLayoutOptions);
            GUI.enabled = true;
            return click;
        }

        #endregion GUI
    }
}