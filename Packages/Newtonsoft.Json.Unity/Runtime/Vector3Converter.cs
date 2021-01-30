using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Unity
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public static Vector3Converter Default { get; } = new Vector3Converter();

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["x"] = new JValue(value.x),
                ["y"] = new JValue(value.y),
                ["z"] = new JValue(value.z)
            };
            obj.WriteTo(writer);
        }

        public override Vector3 ReadJson(JsonReader reader,
            Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var value = new Vector3
            (
                obj["x"].ToObject<float>(),
                obj["y"].ToObject<float>(),
                obj["z"].ToObject<float>()
            );
            return value;
        }
    }
}