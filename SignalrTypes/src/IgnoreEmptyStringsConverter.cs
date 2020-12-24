using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Septa.AspNetCore.SignalRTypes
{
    public class IgnoreEmptyStringsConverter : JsonConverter
    {
        #region Methods

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            var theValue = reader.Value?.ToString();

            return !string.IsNullOrWhiteSpace(theValue) ? theValue : null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!string.IsNullOrWhiteSpace(value.ToString()))
            {
                JToken token = JToken.FromObject(value.ToString(), serializer);
                token.WriteTo(writer);
                return;
            }

            writer.WriteNull();
        }

        #endregion
    }

}
