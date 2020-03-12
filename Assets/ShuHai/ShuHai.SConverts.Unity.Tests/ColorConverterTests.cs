using NUnit.Framework;
using ShuHai.Unity;
using UnityEngine;

namespace ShuHai.SConverts.Unity
{
    public class ColorConverterTests : SConverterTests
    {
        [Test]
        public void Convert()
        {
            var c = ColorConverter.Default;
            Convert(c, Color.red, "RGBA(255,0,0,255)");
            Convert(c, Color.white, "RGBA( 255 , 255, 255, 255  \t )");
            Convert(c, Color.black, "RGBA( 00,\t 000, 0, 255)");
            Convert(c, Colors.Purple, "RGBA(128, 0,\t128)");
        }
    }
}