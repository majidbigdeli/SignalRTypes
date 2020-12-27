using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeGeneratorSettings : JsonSchemaGeneratorSettings
    {

        public SignalrTypeGeneratorSettings(JsonSerializerSettings jsonSerializerSettings)
        {
            SerializerSettings = jsonSerializerSettings;
            SchemaType = SchemaType.OpenApi3;
        }
    }


}
