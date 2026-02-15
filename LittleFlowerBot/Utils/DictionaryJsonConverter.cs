using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LittleFlowerBot.Utils
{
    public class DictionaryJsonConverter<TKey, TValue> : JsonConverter<IDictionary<TKey, TValue>> where TKey : Enum
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>))
            {
                return false;
            }

            return typeToConvert.GetGenericArguments()[0] == typeof(TKey);
        }

        public override IDictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionaryWithStringKey = (Dictionary<string, TValue>?)JsonSerializer.Deserialize(ref reader, typeof(Dictionary<string, TValue>), options);

            var dictionary = new Dictionary<TKey, TValue>();

            if (dictionaryWithStringKey == null)
            {
                return dictionary;
            }

            foreach (var kvp in dictionaryWithStringKey)
            {
                dictionary.Add((TKey)Enum.ToObject(typeof(TKey), Int32.Parse(kvp.Key)), kvp.Value);
            }

            return dictionary;
        }

        public override void Write(Utf8JsonWriter writer, IDictionary<TKey, TValue> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToDictionary(k => (Convert.ToInt32((object?) k.Key)).ToString(), v => v.Value));
        }
    }
}