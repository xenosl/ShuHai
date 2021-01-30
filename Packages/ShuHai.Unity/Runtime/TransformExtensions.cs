using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Provides commonly used extension methods for <see cref="Transform" />.
    /// </summary>
    public static class TransformExtensions
    {
        #region Position

        /// <summary>
        ///     Set position without changing its z value.
        /// </summary>
        public static void SetPosition2D(this Transform self, Vector2 value)
        {
            self.position = new Vector3(value.x, value.y, self.position.z);
        }

        /// <summary>
        ///     Set local position without changing its z value.
        /// </summary>
        public static void SetLocalPosition2D(this Transform self, Vector2 value)
        {
            self.localPosition = new Vector3(value.x, value.y, self.localPosition.z);
        }

        #endregion Position

        #region Scale

        public static void SetLocalScale(this Transform self, float value)
        {
            self.localScale = new Vector3(value, value, value);
        }

        public static void SetLossyScale(this Transform self, Vector2 value)
        {
            var parent = self.parent;
            if (!parent)
            {
                self.localScale = value;
                return;
            }

            var lossyOfParents = Vector2.one;
            while (parent)
            {
                lossyOfParents = Vector2.Scale(lossyOfParents, parent.localScale);
                parent = parent.parent;
            }
            Vector3 scale = VectorExtensions.InverseScale(value, lossyOfParents);
            scale.z = self.localScale.z;
            self.localScale = scale;
        }

        public static void SetLossyScale(this Transform self, Vector3 value)
        {
            var parent = self.parent;
            var lossyOfParents = Vector3.one;
            while (parent)
            {
                lossyOfParents = Vector3.Scale(lossyOfParents, parent.localScale);
                parent = parent.parent;
            }
            self.localScale = VectorExtensions.InverseScale(value, lossyOfParents);
        }

        #endregion Scale

        #region Hierarchy

        public static IEnumerable<Transform> EnumerateParents(this Transform self)
        {
            UnityEnsure.Argument.NotNull(self, nameof(self));

            var p = self;
            while (p)
            {
                p = p.parent;
                yield return p;
            }
        }

        public static IEnumerable<Transform> EnumerateChildren(this Transform self)
        {
            UnityEnsure.Argument.NotNull(self, nameof(self));

            for (int i = 0; i < self.childCount; ++i)
                yield return self.GetChild(i);
        }

        public static Transform FindOrCreateChild(this Transform self, string name)
        {
            UnityEnsure.Argument.NotNull(self, nameof(self));

            var t = self.Find(name);
            if (!t)
            {
                t = new GameObject(name).transform;
                t.parent = self;
            }
            return t;
        }

        public static void DestroyChildren(this Transform self,
            Func<Transform, bool> predicate = null, Action<Transform> beforeDestroy = null)
        {
            UnityEnsure.Argument.NotNull(self, nameof(self));

            var destroyList = self.OfType<Transform>().Where(c => predicate == null || predicate(c)).ToArray();
            foreach (var child in destroyList)
            {
                beforeDestroy?.Invoke(child);
                UnityObjectUtil.Destroy(child.gameObject);
            }
        }

        /// <summary>
        ///     Build a string represents the path to current transform in the transform hierarchy.
        /// </summary>
        /// <param name="self">The transform instance to build with.</param>
        /// <param name="separator">Separator between each transform name in the result string.</param>
        public static string MakeHierarchicName(this Transform self, string separator = "/")
        {
            var builder = new StringBuilder(self.name);
            while (self.parent)
            {
                self = self.parent;
                builder.InsertHead(separator).InsertHead(self.name);
            }
            return builder.ToString();
        }

        #endregion Hierarchy

        #region Component

        public static T GetOrAddComponent<T>(this Transform self)
            where T : Component
        {
            return self.gameObject.GetOrAddComponent<T>();
        }

        /// <summary>
        ///     Get the specified type of component attached to current <see cref="Transform" /> or throws
        ///     <see cref="MissingComponentException" /> if not found.
        /// </summary>
        /// <typeparam name="T">Type of the specified component</typeparam>
        /// <param name="self">Instance of which the specified component is attached to.</param>
        public static T EnsureComponent<T>(this Transform self)
            where T : Component
        {
            return (T)self.gameObject.EnsureComponentImpl(typeof(T));
        }

        public static Component EnsureComponent(this Transform self, Type type)
        {
            return self.gameObject.EnsureComponentImpl(type);
        }

        /// <summary>
        ///     Get the specified type of component attached to current <see cref="Transform" /> or any of its parents,
        ///     Throws <see cref="MissingComponentException" /> if not found.
        /// </summary>
        /// <typeparam name="T">Type of the specified component</typeparam>
        /// <param name="self">Instance of which the specified component is attached to.</param>
        /// <returns></returns>
        public static Component EnsureComponentInParent<T>(this Transform self)
            where T : Component
        {
            return (T)self.gameObject.EnsureComponentInParentImpl(typeof(T));
        }

        public static Component EnsureComponentInParent(this Transform self, Type type)
        {
            return self.gameObject.EnsureComponentInParentImpl(type);
        }

        public static T FindComponentInChildren<T>(
            this Transform self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            Ensure.Argument.NotNull(self, nameof(self));

            if (findInSelf)
            {
                var c = FindComponent(self, match);
                if (c)
                    return c;
            }

            for (var i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                {
                    var cc = FindComponent(child, match);
                    if (cc)
                        return cc;
                }

                var ccc = FindComponentInChildren(child, match, findInSelf);
                if (ccc)
                    return ccc;
            }
            return null;
        }

        private static T FindComponent<T>(Transform transform, Func<T, bool> match)
            where T : Component
        {
            var components = transform.GetComponents<T>();
            return match != null ? components.FirstOrDefault(match) : components.FirstOrDefault();
        }

        public static List<T> FindComponentsInChildren<T>(
            this Transform self, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            var result = new List<T>();
            FindComponentsInChildren(self, result, match, findInSelf);
            return result;
        }

        /// <remarks>Similar with GetComponentsInChildren when match is null.</remarks>
        public static void FindComponentsInChildren<T>(this Transform self,
            ICollection<T> result, Func<T, bool> match = null, bool findInSelf = true)
            where T : Component
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(result, "result");

            if (findInSelf)
                result.AddRange(FindComponents(self, match));

            for (var i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                    result.AddRange(FindComponents(child, match));
                FindComponentsInChildren(child, result, match, findInSelf);
            }
        }

        private static IEnumerable<T> FindComponents<T>(Transform transform, Func<T, bool> match)
            where T : Component
        {
            var components = transform.GetComponents<T>();
            return match != null ? components.Where(match) : components;
        }

        public static T FindComponentInChild<T>(
            this Transform self, string path) where T : Component
        {
            var child = self.Find(path);
            return child ? child.GetComponent<T>() : null;
        }

        /// <summary>
        ///     Equivalent to <see cref="GameObjectExtensions.DestroyComponents{T}(GameObject)" />.
        /// </summary>
        public static int DestroyComponents<T>(this Transform self)
            where T : Component
        {
            return self.gameObject.DestroyComponents<T>();
        }

        #endregion Component

        #region GameObject

        public static GameObject FindGameObjectInChildren(
            this Transform self, Predicate<GameObject> match, bool findInSelf = true)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(match, "match");

            if (findInSelf)
            {
                var selfObj = self.gameObject;
                if (match(selfObj))
                    return selfObj;
            }

            for (var i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                {
                    var childObj = child.gameObject;
                    if (match(childObj))
                        return childObj;
                }

                var obj = FindGameObjectInChildren(child, match, findInSelf);
                if (obj && match(obj))
                    return obj;
            }
            return null;
        }

        public static List<GameObject> FindGameObjectsInChildren(
            this Transform self, Predicate<GameObject> match, bool findInSelf = true)
        {
            var result = new List<GameObject>();
            FindGameObjectsInChildren(self, result, match, findInSelf);
            return result;
        }

        public static void FindGameObjectsInChildren(this Transform self,
            ICollection<GameObject> result, Predicate<GameObject> match, bool findInSelf = true)
        {
            Ensure.Argument.NotNull(self, nameof(self));
            Ensure.Argument.NotNull(match, "match");
            Ensure.Argument.NotNull(result, "result");

            if (findInSelf)
            {
                var selfObj = self.gameObject;
                if (match(selfObj))
                    result.Add(selfObj);
            }

            for (var i = 0; i < self.childCount; ++i)
            {
                var child = self.GetChild(i);
                if (!findInSelf)
                {
                    var obj = child.gameObject;
                    if (match(obj))
                        result.Add(obj);
                }
                FindGameObjectsInChildren(child, result, match, findInSelf);
            }
        }

        #endregion GameObject
    }
}
