using System;

namespace SMS_Gate.Model
{
    public class Client
    {
        public int id { get; set; }
        public string phone_num { get; set; }
        public string text { get; set; }
        public int status { get; set; }
        public DateTime created { get; set; }
        public DateTime sended { get; set; }

    }
}
