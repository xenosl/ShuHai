using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace ShuHai.Localization
{
    public sealed class Language
    {
        public readonly CultureInfo CultureInfo;

        private Language(CultureInfo cultureInfo) { CultureInfo = cultureInfo; }

        #region Texts

        public IReadOnlyDictionary<string, string> Texts => _texts;

        /// <summary>
        ///     Get the text string representing the specified id.
        /// </summary>
        /// <param name="id">ID of the text string.</param>
        /// <returns>The text string representing the specified id if found; otherwise, the id itself.</returns>
        public string GetText(string id) { return _texts.TryGetValue(id, out var text) ? text : id; }

        /// <summary>
        ///     Load translation texts from the specified CSV string, add the loaded texts to current language instance.
        /// </summary>
        /// <param name="csv">The CSV string to load from.</param>
        /// <returns>Number of texts loaded.</returns>
        public int LoadTextsFromCSV(string csv)
        {
            int loadCount = 0;
            using (var reader = new CsvReader(new StringReader(csv)))
            {
                var dataCollection = reader.GetRecords<CSVData>();
                foreach (var data in dataCollection)
                {
                    _texts[data.ID] = data.Text;
                    loadCount++;
                }
            }
            return loadCount;
        }

        private readonly Dictionary<string, string> _texts = new Dictionary<string, string>();

        private struct CSVData
        {
#pragma warning disable CS0649
            public string ID;
            public string Text;
#pragma warning restore CS0649
        }

        #endregion Texts

        #region Instances

        public static Language Current
        {
            get => _current;
            set
            {
                if (value == _current)
                    return;
                _current = value ?? _current;
            }
        }

        private static Language _current;

        public static Language Get(string name)
        {
            try
            {
                if (!_instances.TryGetValue(name, out var lang))
                {
                    lang = new Language(new CultureInfo(name));
                    _instances.Add(name, lang);

                    if (Current == null)
                        Current = lang;
                }
                return lang;
            }
            catch (CultureNotFoundException)
            {
                return null;
            }
        }

        private static readonly Dictionary<string, Language> _instances = new Dictionary<string, Language>();

        #endregion Instances
    }
}