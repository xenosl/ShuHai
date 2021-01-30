using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShuHai.I18N.Json
{
    public static class LanguageExtensions
    {
        public static int LoadTextsFromJson(this Language self, string json, bool overwrite = true)
        {
            var conflicts = new List<TextData>();
            var loadedCount = LoadTextsFromJson(self, json, conflicts);
            if (!overwrite)
                return loadedCount;

            foreach (var data in conflicts)
            {
                self.Texts[data.Key] = data.Text;
                loadedCount++;
            }
            return loadedCount;
        }

        public static int LoadTextsFromJson(this Language self, string json, List<TextData> conflicts)
        {
            int loadedCount = 0;
            var dataList = JsonConvert.DeserializeObject<TextData[]>(json);
            var texts = self.Texts;
            foreach (var data in dataList)
            {
                if (texts.TryGetValue(data.Key, out var existingText) && existingText != data.Text)
                {
                    conflicts?.Add(data);
                    continue;
                }
                texts.Add(data.Key, data.Text);
                loadedCount++;
            }
            return loadedCount;
        }
    }
}
