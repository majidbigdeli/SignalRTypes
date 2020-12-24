using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SignalrTypes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SignalrTypeOperationType
    {
        Sync,

        Observable
    }

}
