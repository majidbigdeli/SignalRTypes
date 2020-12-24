using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.AspNetCore.SignalRTypes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SignalrTypeOperationType
    {
        Sync,

        Observable
    }

}
