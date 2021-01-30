using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Unity
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public static Vector2Converter Default { get; } = new Vector2Converter();

        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["x"] = new JValue(value.x),
                ["y"] = new JValue(value.y)
            };
            obj.WriteTo(writer);
        }

        public override Vector2 ReadJson(JsonReader reader,
            Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var value = new Vector2
            (
                obj["x"].ToObject<float>(),
                obj["y"].ToObject<float>()
            );
            return value;
        }
    }
}