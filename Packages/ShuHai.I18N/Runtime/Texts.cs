namespace ShuHai.I18N
{
    public static class Texts
    {
        public static string Get(string key) { return Language.Current.GetText(key); }
    }
}
