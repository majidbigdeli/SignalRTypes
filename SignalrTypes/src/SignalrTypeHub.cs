using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.AspNetCore.SignalRTypes
{
    public class SignalrTypeHub
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [DefaultValue("")]
        [JsonProperty("description",DefaultValueHandling= DefaultValueHandling.Ignore,NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("operations")]
        public IDictionary<string, SignalrTypeOperation> Operations { get; } = new Dictionary<string, SignalrTypeOperation>();

        [JsonProperty("callbacks")]
        public IDictionary<string, SignalrTypeOperation> Callbacks { get; } = new Dictionary<string, SignalrTypeOperation>();
    }

}
