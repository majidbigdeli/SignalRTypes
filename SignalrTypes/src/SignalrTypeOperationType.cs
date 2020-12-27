using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Septa.AspNetCore.SignalRTypes
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SignalrTypeOperationType
    {
        Sync,
        Observable
    }

}
