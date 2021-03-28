using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ShuHai.Reflection;
using UnityEngine;

namespace ShuHai.I18N.Unity
{
    using StringPair = KeyValuePair<string, string>;
    using TextParser = Func<string, IEnumerable<KeyValuePair<string, string>>>;

    [ExecuteAlways]
    public class LanguageLoader : ScriptableObject
    {
        public LanguageManifest ManifestToLoad;

        private void LoadManifest()
        {
            if (!ManifestToLoad)
                return;
            var assetDict = ManifestToLoad.LanguageAssets;
            if (CollectionUtil.IsNullOrEmpty(assetDict))
                return;

            var parser = TextParsers.FirstOrDefault();
            if (parser == null)
                return;

            foreach (var (langName, asset) in assetDict)
            {
                foreach (var textAsset in asset.TextAssets)
                {
                    try
                    {
                        var lang = Language.GetOrCreate(langName);
                        var textPairs = parser(textAsset.text);
                        lang.Texts.AddRange(textPairs);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
        }

        #region Unity Events

        private void Awake() { LoadManifest(); }

        #endregion Unity Events

        #region Text Parsers

        public static IReadOnlyCollection<TextParser> TextParsers { get; private set; }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
#else
        [RuntimeInitializeOnLoadMethod]
#endif
        private static void CollectTextParsers()
        {
            if (!MethodCache.StaticMethodsByAttributeType.TryGetValue(
                typeof(LanguageTextParserAttribute), out var methods))
                return;

            TextParsers = methods.Where(IsParserMethod)
                .Select(m => (TextParser)m.CreateDelegate(typeof(TextParser))).ToList();
        }

        private static bool IsParserMethod(MethodInfo method)
        {
            if (!method.IsClosedConstructedMethod())
                return false;

            if (method.ReturnType != typeof(IEnumerable<StringPair>))
                return false;

            var parameters = method.GetParameters();
            if (parameters.Length != 1)
                return false;
            if (parameters[0].ParameterType != typeof(string))
                return false;

            return true;
        }

        #endregion Text Parsers
    }
}
