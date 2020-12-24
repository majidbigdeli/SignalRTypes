using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using System.ComponentModel;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeParameter : JsonSchema
    {
        //
        // Summary:
        //     Gets or sets the description.
        [DefaultValue("")]
        [JsonProperty("description", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public override string Description { get; set; }


    }

}
