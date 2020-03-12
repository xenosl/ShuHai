using System;
using UnityEngine.UIElements;

namespace ShuHai.Unity
{
    public static class VisualElementExtensions
    {
        public static Button QueryButton(this VisualElement self,
            string name = null, string[] classes = null, Action onClick = null)
        {
            return Query<Button>(self, name, classes, b => b.clickable.clicked += onClick);
        }

        public static Button QueryButton(this VisualElement self,
            string name = null, string[] classes = null, Action<EventBase> onClick = null)
        {
            return Query<Button>(self, name, classes, b => b.clickable.clickedWithEventInfo += onClick);
        }

        private static T Query<T>(this VisualElement self, string name, string[] classes, Action<T> done)
            where T : VisualElement
        {
            var e = self.Q<T>(name, classes);
            if (e != null)
                done?.Invoke(e);
            return e;
        }
    }
}
