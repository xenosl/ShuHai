using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShuHai.Unity
{
    public static class SceneExtensions
    {
        #region Finds

        public static T FindComponent<T>(this Scene self, Func<T, bool> match = null)
            where T : Component
        {
            EnsureLoaded(self);

            var transforms = self.GetRootGameObjects().Select(o => o.transform);
            foreach (var t in transforms)
            {
                var c = t.FindComponentInChildren(match);
                if (c != null)
                    return c;
            }
            return null;
        }

        public static List<T> FindComponents<T>(this Scene self, Func<T, bool> match = null)
            where T : Component
        {
            EnsureLoaded(self);

            var result = new List<T>();
            FindComponents(self, result, match);
            return result;
        }

        public static void FindComponents<T>(
            this Scene self, ICollection<T> result, Func<T, bool> match = null)
            where T : Component
        {
            EnsureLoaded(self);

            self.GetRootGameObjects().Select(o => o.transform)
                .ForEach(t => t.FindComponentsInChildren(result, match));
        }

        public static GameObject FindGameObject(this Scene self, Predicate<GameObject> match)
        {
            EnsureLoaded(self);
            Ensure.Argument.NotNull(match, nameof(match));

            var transforms = self.GetRootGameObjects().Select(o => o.transform);
            foreach (var t in transforms)
            {
                var obj = t.FindGameObjectInChildren(match);
                if (obj != null)
                    return obj;
            }
            return null;
        }

        public static List<GameObject> FindGameObjects(this Scene self, Predicate<GameObject> match)
        {
            EnsureLoaded(self);
            Ensure.Argument.NotNull(match, "match");

            var result = new List<GameObject>();
            FindGameObjects(self, result, match);
            return result;
        }

        public static void FindGameObjects(this Scene self,
            ICollection<GameObject> result, Predicate<GameObject> match)
        {
            EnsureLoaded(self);
            Ensure.Argument.NotNull(match, "match");

            var transforms = self.GetRootGameObjects().Select(o => o.transform);
            foreach (var t in transforms)
                t.FindGameObjectsInChildren(result, match);
        }

        private static void EnsureLoaded(Scene self)
        {
            if (!self.isLoaded)
                throw new ArgumentException("Scene is not loaded", nameof(self));
        }

        #endregion Finds
    }
}
