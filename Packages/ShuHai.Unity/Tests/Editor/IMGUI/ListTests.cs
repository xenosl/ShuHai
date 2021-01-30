using NUnit.Framework;
using UnityEditor;

namespace ShuHai.Unity.Editor.IMGUI
{
    public class ListTests
    {
        private class Item : IControl
        {
            private static int NextID = 1;

            public readonly int ID;

            public string Name;

            public Item() : this(typeof(Item).Name) { }

            public Item(string name)
            {
                ID = NextID++;
                Name = name;
            }

            public void OnGUI() { EditorGUILayout.LabelField(Name); }
        }

        [Test]
        public void Items()
        {
            var list = new List<Item>();
            ItemsAsserts(list, 0, 0, Index.Invalid, Index.Invalid, Index.Invalid);

            {
                list.Add(new Item("0"));
                ItemsAsserts(list, 1, 1, 0, 0, 0);

                list.Add(new[] { new Item("1"), new Item("2") });
                ItemsAsserts(list, 3, 1, 0, 0, 2);

                list.RemoveAt(1);
                ItemsAsserts(list, 2, 1, 0, 0, 1);
                Assert.AreEqual("0", list[0].Name);
                Assert.AreEqual("2", list[1].Name);

                list.Clear();
                ItemsAsserts(list, 0, 0, Index.Invalid, Index.Invalid, Index.Invalid);
            }
            {
                int count = list.ItemCountPerPage;
                for (int i = 0; i < count; ++i)
                    list.Add(new Item(i.ToString()));
                ItemsAsserts(list, count, 1, 0, 0, count - 1);

                list.Clear();
                ItemsAsserts(list, 0, 0, Index.Invalid, Index.Invalid, Index.Invalid);
            }
            {
                const int count = 47;
                for (var i = 0; i < count; ++i)
                    list.Add(new Item(i.ToString()));
                ItemsAsserts(list, count, 5, 0, 0, 9);

                list.PageIndex = 2;
                ItemsAsserts(list, count, 5, 2, 20, 29);
                
                list.PageIndex = 4;
                ItemsAsserts(list, count, 5, 4, 40, 46);
                
                list.Clear();
                ItemsAsserts(list, 0, 0, Index.Invalid, Index.Invalid, Index.Invalid);
            }
        }

        private static void ItemsAsserts(List<Item> list,
            int itemCount, int pageCount, int pageIndex, int firstItemIndex, int lastItemIndex)
        {
            Assert.AreEqual(itemCount, list.Count);
            Assert.AreEqual(pageCount, list.PageCount);
            Assert.AreEqual(pageIndex, list.PageIndex);
            Assert.AreEqual(firstItemIndex, list.FirstItemIndex);
            Assert.AreEqual(lastItemIndex, list.LastItemIndex);
        }
    }
}