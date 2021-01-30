using System.Globalization;
using NUnit.Framework;

namespace ShuHai.I18N.Test
{
    public class LanguageTest
    {
        [Test]
        public void DefaultLanguageCultureShouldSameAsUI()
        {
            Assert.AreEqual(CultureInfo.CurrentUICulture, Language.Default.CultureInfo);
        }
    }
}
