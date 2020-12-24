using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using System.ComponentModel;

namespace SignalrTypes
{
    /// <summary>The web service description.</summary>
    public class SignalrTypeInfo : JsonExtensionObject
    {
        /// <summary>Gets or sets the title.</summary>
        [JsonProperty(PropertyName = "title", Required = Required.Always,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public string Title { get; set; } = "SignalrType specification";

        /// <summary>Gets or sets the description.</summary>
        [DefaultValue("")]
        [JsonProperty(PropertyName = "description", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>Gets or sets the terms of service.</summary>
        [DefaultValue("")]
        [JsonProperty(PropertyName = "termsOfService", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string TermsOfService { get; set; }

        /// <summary>Gets or sets the API version.</summary>
        [JsonProperty(PropertyName = "version", Required = Required.Always, DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; } = "1.0.0";
    }

}
