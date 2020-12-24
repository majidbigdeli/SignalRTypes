using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using System.Collections.Generic;
using System.ComponentModel;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeOperation
    {
        [DefaultValue("")]
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("parameters")]
        public IDictionary<string, SignalrTypeParameter> Parameters { get; } = new Dictionary<string, SignalrTypeParameter>();

        [JsonProperty("returntype", NullValueHandling = NullValueHandling.Ignore)]
        public JsonSchema ReturnType { get; set; }

        [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SignalrTypeOperationType Type { get; set; }
    }

}
