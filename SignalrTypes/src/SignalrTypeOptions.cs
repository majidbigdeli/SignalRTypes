using System;
using System.Collections.Generic;

namespace Septa.AspNetCore.SignalRTypes
{
    public class SignalrTypeOptions
    {
        public string RoutePath { get; set; }
        public Dictionary<string, Type> HubServices { get; set; }
    }

}
