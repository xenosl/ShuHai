using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Newtonsoft.Json.Unity
{
    public class QuaternionConverter : JsonConverter<Quaternion>
    {
        public static QuaternionConverter Default { get; } = new QuaternionConverter();

        public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["x"] = new JValue(value.x),
                ["y"] = new JValue(value.y),
                ["z"] = new JValue(value.z),
                ["w"] = new JValue(value.w)
            };
            obj.WriteTo(writer);
        }

        public override Quaternion ReadJson(JsonReader reader,
            Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);

            var value = new Quaternion
            (
                obj["x"].ToObject<float>(),
                obj["y"].ToObject<float>(),
                obj["z"].ToObject<float>(),
                obj["w"].ToObject<float>()
            );
            return value;
        }
    }
}