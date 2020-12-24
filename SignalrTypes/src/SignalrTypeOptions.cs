using System;
using System.Collections.Generic;

namespace SignalrTypes
{
    public class SignalrTypeOptions
    {
        public string RoutePath { get; set; }
        public Dictionary<string, Type> Hubs { get; set; }
    }

}
