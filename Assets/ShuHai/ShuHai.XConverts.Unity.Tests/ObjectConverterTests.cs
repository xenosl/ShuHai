using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace ShuHai.XConverts.Unity.Tests
{
    using UObject = UnityEngine.Object;

    public class ObjectConverterTests
    {
//        [UnityTest]
//        public IEnumerator GameObjectConvert()
//        {
//            var c = GameObjectConverter.Default;
//            var obj = new GameObject("Test");
//            c.ConvertedEqual(obj, null, GameObjectEqualityComparer.Default);
//            yield return null;
//        }

        [Test]
        public void GameObjectConvert()
        {
            var c = GameObjectConverter.Default;
            var obj = new GameObject("Test");
            c.ConvertedEqual(obj, null, GameObjectEqualityComparer.Default);
        }

        private class ObjectEqualityComparer : IEqualityComparer<UObject>
        {
            public virtual bool Equals(UObject x, UObject y) { return x.name == y.name; }
            public virtual int GetHashCode(UObject obj) { return obj ? obj.GetHashCode() : 0; }
        }

        private class ComponentEqualityComparer : ObjectEqualityComparer, IEqualityComparer<Component>
        {
            public virtual bool Equals(Component x, Component y) { return base.Equals(x, y); }
            public virtual int GetHashCode(Component obj) { return base.GetHashCode(obj); }
        }

        private class TransformEqualityComparer : ComponentEqualityComparer, IEqualityComparer<Transform>
        {
            public static TransformEqualityComparer Default { get; } = new TransformEqualityComparer();

            public virtual bool Equals(Transform x, Transform y)
            {
                return x.position == y.position
                    && x.rotation == y.rotation
                    && x.localScale == y.localScale;
            }

            public virtual int GetHashCode(Transform obj) { return base.GetHashCode(obj); }
        }

        private sealed class GameObjectEqualityComparer : ObjectEqualityComparer, IEqualityComparer<GameObject>
        {
            public static GameObjectEqualityComparer Default { get; } = new GameObjectEqualityComparer();

            public override bool Equals(Object x, Object y) { return Equals((GameObject)x, (GameObject)y); }

            public bool Equals(GameObject x, GameObject y)
            {
                return x.name == y.name
                    && TransformEqualityComparer.Default.Equals(x.transform, y.transform)
                    && x.activeSelf == y.activeSelf
                    && x.layer == y.layer
                    && x.isStatic == y.isStatic
                    && x.tag == y.tag;
            }

            public int GetHashCode(GameObject obj) { return base.GetHashCode(obj); }
        }
    }
}