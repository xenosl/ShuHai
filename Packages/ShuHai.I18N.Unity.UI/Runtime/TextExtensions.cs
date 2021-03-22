using UnityEngine.UI;

namespace ShuHai.I18N.Unity.UI
{
    public static class TextExtensions
    {
        public static void LocalizeText(this Text self, Language language, string key)
        {
            Ensure.Argument.NotNull(language, nameof(language));
            self.text = language.GetText(key);
        }

        public static void LocalizeText(this Text self, string key) { LocalizeText(self, Language.Current, key); }
    }
}
