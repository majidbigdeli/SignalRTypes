﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.SignalRTypes
{
    public class SignalrTypeGeneratorSettings : JsonSchemaGeneratorSettings
    {

        public SignalrTypeGeneratorSettings()
        {
            SerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Converters = new List<JsonConverter> { new IgnoreEmptyStringsConverter(), new StringEnumConverter() }
            };

            SchemaType = SchemaType.OpenApi3;


        }
    }


}
