using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShuHai.Unity
{
    public static class GameObjectExtensions
    {
        #region Hierarchy

        public static bool IsChildOf(this GameObject self, GameObject other)
        {
            UnityEnsure.Argument.NotNull(other, nameof(other));
            return self.transform.IsChildOf(other.transform);
        }

        /// <summary>
        ///     Enumerate all parent game objects of current instance from its parent to root.
        /// </summary>
        public static IEnumerable<GameObject> EnumerateParents(this GameObject self)
        {
            var p = self.transform;
            while (p)
            {
                p = p.parent;
                yield return p.gameObject;
            }
        }

        public static IEnumerable<GameObject> EnumerateChildren(this GameObject self)
        {
            UnityEnsure.Argument.NotNull(self, nameof(self));

            var t = self.transform;
            for (int i = 0; i < t.childCount; ++i)
                yield return t.GetChild(i).gameObject;
        }

        public static GameObject FindOrCreateChild(this GameObject self, string name)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.transform.FindOrCreateChild(name).gameObject;
        }

        public static void DestroyChildren(this GameObject self,
            Func<Transform, bool> predicate = null, Action<GameObject> beforeDestroy = null)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            Action<Transform> bd = null;
            if (beforeDestroy != null)
                bd = t => beforeDestroy(t.gameObject);
            self.transform.DestroyChildren(predicate, bd);
        }

        public static void DestroyChildren(this GameObject self, Func<GameObject, bool> predicate)
        {
            Ensure.Argument.NotNull(self, nameof(self));

            Func<Transform, bool> p = null;
            if (predicate != null)
                p = (t) => predicate(t.gameObject);
            self.transform.DestroyChildren(p);
        }

        public static string MakeHierarchicName(this GameObject self, string separator = "/")
        {
            return self.transform.MakeHierarchicName(separator);
        }

        #endregion Hierarchy

        #region Components

        public static T GetOrAddComponent<T>(this GameObject self)
            where T : Component
        {
            return (T)GetOrAddComponentImpl(self, typeof(T));
        }

        public static Component GetOrAddComponent(this GameObject self, Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<Component>(type, nameof(type));
            return GetOrAddComponentImpl(self, type);
        }

        internal static Component GetOrAddComponentImpl(this GameObject self, Type type)
        {
            var c = self.GetComponent(type);
            if (!c)
                c = self.AddComponent(type);
            return c;
        }

        /// <summary>
        ///     Get the specified type of component attached to current <see cref="GameObject" /> or throws
        ///     <see cref="MissingComponentException" /> if not found.
        /// </summary>
        /// <typeparam name="T">Type of the specified component</typeparam>
        /// <param name="self">Instance of which the specified component is attached to.</param>
        public static T EnsureComponent<T>(this GameObject self)
            where T : Component
        {
            return (T)EnsureComponentImpl(self, typeof(T));
        }

        public static Component EnsureComponent(this GameObject self, Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<Component>(type, nameof(type));
            return EnsureComponentImpl(self, type);
        }

        internal static Component EnsureComponentImpl(this GameObject self, Type type)
        {
            var c = self.GetComponent(type);
            if (!c)
                throw new MissingComponentException(self.MakeHierarchicName(), type);
            return c;
        }

        /// <summary>
        ///     Get the specified type of component attached to current <see cref="GameObject" /> or any of its parents,
        ///     Throws <see cref="MissingComponentException" /> if not found.
        /// </summary>
        /// <typeparam name="T">Type of the specified component</typeparam>
        /// <param name="self">Instance of which the specified component is attached to.</param>
        /// <returns></returns>
        public static Component EnsureComponentInParent<T>(this GameObject self)
            where T : Component
        {
            return (T)EnsureComponentInParentImpl(self, typeof(T));
        }

        public static Component EnsureComponentInParent(this GameObject self, Type type)
        {
            Ensure.Argument.NotNull(type, nameof(type));
            Ensure.Argument.Is<Component>(type, nameof(type));
            return EnsureComponentInParentImpl(self, type);
        }

        internal static Component EnsureComponentInParentImpl(this GameObject self, Type type)
        {
            var c = self.GetComponentInParent(type);
            if (!c)
                throw new MissingComponentException(self.MakeHierarchicName(), type);
            return c;
        }

        public static T FindComponentInChildren<T>(
            this GameObject self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.transform.FindComponentInChildren(match, findInSelf);
        }

        public static List<T> FindComponentsInChildren<T>(
            this GameObject self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.transform.FindComponentsInChildren(match, findInSelf);
        }

        public static void FindComponentsInChildren<T>(this GameObject self,
            ICollection<T> result, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            Ensure.Argument.NotNull(self, nameof(self));
            self.transform.FindComponentsInChildren(result, match, findInSelf);
        }

        public static T EnsureComponentInChild<T>(this GameObject self, string path) where T : Component
        {
            var c = self.transform.FindComponentInChild<T>(path);
            if (!c)
                throw new MissingComponentException(self.MakeHierarchicName(), typeof(T));
            return c;
        }

        public static T FindComponentInChild<T>(this GameObject self, string path) where T : Component
        {
            return self.transform.FindComponentInChild<T>(path);
        }

        /// <summary>
        ///     Destroy all components of the specified type attached to current <see cref="GameObject" />.
        /// </summary>
        /// <param name="self">The <see cref="GameObject" /> instance components attached to.</param>
        /// <typeparam name="T">Type of components to destroy.</typeparam>
        /// <returns>Number of components destroyed.</returns>
        public static int DestroyComponents<T>(this GameObject self)
            where T : Component
        {
            var components = self.GetComponents<T>();
            foreach (var component in components)
                UnityObjectUtil.Destroy(component);
            return components.Length;
        }

        #endregion Components

        #region GameObject

        public static GameObject FindGameObjectInChildren(
            this GameObject self, Predicate<GameObject> match, bool findInSelf = true)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.transform.FindGameObjectInChildren(match, findInSelf);
        }

        public static List<GameObject> FindGameObjectsInChildren(
            this GameObject self, Predicate<GameObject> match, bool findInSelf = true)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            return self.transform.FindGameObjectsInChildren(match, findInSelf);
        }

        public static void FindGameObjectsInChildren(this GameObject self,
            Predicate<GameObject> match, ICollection<GameObject> result, bool findInSelf = true)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            self.transform.FindGameObjectsInChildren(result, match, findInSelf);
        }

        #endregion GameObject
    }
}
