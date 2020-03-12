using System;
using UnityEngine;

namespace ShuHai.Unity
{
    public static class UnityThrows
    {
        #region MissingComponent

        public static void MissingComponent<T>() { MissingComponent(typeof(T), string.Empty); }

        public static void MissingComponent<T>(string attachTarget)
            where T : Component
        {
            MissingComponent(typeof(T), attachTarget);
        }

        public static void MissingComponent(Type componentType) { MissingComponent(componentType, string.Empty); }

        public static void MissingComponent(Type componentType, string attachTarget)
        {
            var message = string.IsNullOrEmpty(attachTarget)
                ? $"{componentType.Name} not found."
                : $"There is no {componentType.Name} attached to {attachTarget}.";
            throw new MissingComponentException(message);
        }

        public static void MissingComponent<T>(GameObject attachTarget)
            where T : Component
        {
            MissingComponent(typeof(T), attachTarget);
        }

        public static void MissingComponent(Type componentType, GameObject attachTarget)
        {
            MissingComponent(componentType, attachTarget.MakeHierarchicName());
        }

        #endregion MissingComponent
    }
}
