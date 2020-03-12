using System;

namespace ShuHai.Localization
{
    public static class Texts
    {
        public static string Get(string id)
        {
            var lang = Language.Current;
            if (lang == null)
                throw new InvalidOperationException("Language not loaded.");
            return lang.GetText(id);
        }
    }
}