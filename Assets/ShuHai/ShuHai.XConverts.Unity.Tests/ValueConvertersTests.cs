using NUnit.Framework;
using ShuHai.XConverts.Converters;
using UnityEngine;

namespace ShuHai.XConverts.Unity.Tests
{
    public class ValueConvertersTests
    {
        [Test]
        public void Vector2Convert()
        {
            var c = Vector2Converter.Default;
            c.ConvertedEqual(Vector2.zero);
            c.ConvertedEqual(Vector2.one);
            c.ConvertedEqual(new Vector2(0.2f, -951.1782f));
            //c.ConvertedEqual( new Vector2(0.2f, float.MinValue), bytesForFp);
        }

        [Test]
        public void Vector3Convert()
        {
            var c = Vector3Converter.Default;
            ;
            c.ConvertedEqual(Vector3.zero);
            c.ConvertedEqual(Vector3.one);
            c.ConvertedEqual(new Vector3(0.2f, -951.1782f, 22f));
            //c.ConvertedEqual( new Vector3(0.2f, 22f, -231.1786522122223098234975622f), bytesForFp);
        }

        [Test]
        public void Vector4Convert()
        {
            var c = Vector4Converter.Default;
            c.ConvertedEqual(Vector4.zero);
            c.ConvertedEqual(Vector4.one);
            c.ConvertedEqual(new Vector4(0.2f, -951.1782f, 22f, 1.12f));
        }

        [Test]
        public void Vector2IntConvert()
        {
            var c = Vector2IntConverter.Default;
            c.ConvertedEqual(Vector2Int.zero);
            c.ConvertedEqual(Vector2Int.one);
            c.ConvertedEqual(new Vector2Int(int.MinValue, int.MaxValue));
        }

        [Test]
        public void Vector3IntConvert()
        {
            var c = Vector3IntConverter.Default;
            c.ConvertedEqual(Vector3Int.zero);
            c.ConvertedEqual(Vector3Int.one);
            c.ConvertedEqual(new Vector3Int(int.MinValue, int.MaxValue, 0));
        }

        [Test]
        public void QuaternionConvert()
        {
            var c = QuaternionConverter.Default;
            c.ConvertedEqual(Quaternion.identity);
            //c.ConvertedEqual( Quaternion.AngleAxis(0.2577662f, Vector3.back), bytesForFp);
            //c.ConvertedEqual( new Quaternion(0.55477f, 0.77f, float.MinValue, 1), bytesForFp);
        }

        [Test]
        public void Matrix4x4Convert()
        {
            var c = Matrix4x4Converter.Default;
            c.ConvertedEqual(Matrix4x4.identity);
            c.ConvertedEqual(Matrix4x4.zero);
            //c.ConvertedEqual( Matrix4x4.Perspective(70f, 1.3334f, 1, 1000));
        }

        [Test]
        public void RectConvert()
        {
            var c = RectConverter.Default;
            c.ConvertedEqual(Rect.zero);
            c.ConvertedEqual(new Rect(0f, 22.43f, 280f, 2244.11f));
        }
    }
}