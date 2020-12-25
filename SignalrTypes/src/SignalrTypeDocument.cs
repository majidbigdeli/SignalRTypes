using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Converters;
using System;
using System.Collections.Generic;

namespace Septa.AspNetCore.SignalRTypes
{
    [JsonConverter(typeof(JsonReferenceConverter))]
    public class SignalrTypeDocument
    {
        private static Lazy<JsonSerializerSettings> _serializerSettings = new Lazy<JsonSerializerSettings>(() => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        });

        /// <summary>Gets or sets the SignalrType specification version being used.</summary>
        [JsonProperty(PropertyName = "SignalrType", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string SignalrType { get; set; } = "1.0.0";

        /// <summary>Gets or sets the metadata about the API.</summary>
        [JsonProperty(PropertyName = "info", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public SignalrTypeInfo Info { get; set; } = new SignalrTypeInfo();

        /// <summary>Gets the exposed SignalR hubs.</summary>
        [JsonProperty("hubs")]
        public IDictionary<string, SignalrTypeHub> Hubs { get; } = new Dictionary<string, SignalrTypeHub>();

        [JsonProperty("definitions")]
        public IDictionary<string, JsonSchema> Definitions { get; } = new Dictionary<string, JsonSchema>();

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, _serializerSettings.Value);
        }
    }

    public class SignalRCallBackAttribute : Attribute
    {
        private readonly Type callBackType;

        public SignalRCallBackAttribute(Type callBackType)
        {
            this.callBackType = callBackType;
        }
    }

}
