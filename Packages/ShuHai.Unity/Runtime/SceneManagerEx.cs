using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShuHai.Unity
{
    /// <summary>
    ///     Extra methods of <see cref="SceneManager" />.
    /// </summary>
    public static class SceneManagerEx
    {
        public static IEnumerable<Scene> Scenes
        {
            get
            {
                for (var i = 0; i < SceneManager.sceneCount; ++i)
                    yield return SceneManager.GetSceneAt(i);
            }
        }

        public static T FindComponent<T>(Func<T, bool> match = null)
            where T : Component
        {
            return Scenes.Select(scene => scene.FindComponent(match)).FirstOrDefault(component => component);
        }

        public static List<T> FindComponents<T>(Func<T, bool> match = null)
            where T : Component
        {
            var result = new List<T>();
            FindComponents(result, match);
            return result;
        }

        public static void FindComponents<T>(ICollection<T> result, Func<T, bool> match = null)
            where T : Component
        {
            foreach (var scene in Scenes)
                scene.FindComponents(result, match);
        }

        public static GameObject FindGameObject(Predicate<GameObject> match)
        {
            return Scenes.Select(s => s.FindGameObject(match)).FirstOrDefault();
        }

        public static void FindGameObjects(ICollection<GameObject> result, Predicate<GameObject> match)
        {
            foreach (var scene in Scenes)
                scene.FindGameObjects(result, match);
        }
    }
}