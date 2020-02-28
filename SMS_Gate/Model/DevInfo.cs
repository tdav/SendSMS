using System.Collections.Generic;

namespace SMS_Gate.Model
{
    public class DevInfo
    {
        public string Description { get; set; }
        public decimal SignalStrength { get; set; }
        public  List< string> Comands { get; set; }
    }
}
