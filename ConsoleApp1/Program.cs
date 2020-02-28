using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = "3g modemdan prep" + DateTime.Now.ToString();
            string ComPort = "COM14";
            string number = "+998998278960";

            //if (args.Length > 0)
            //{
            //    ComPort = args[0];
            //    number = args[1];
            //    text = args[2];
            //}else
            //{
            //    Console.WriteLine("sms_cli /dev/ttyUSB0 +998998278960 text");
            //    return;
            //}

            var modem = new GsmModem.GsmModem(ComPort);
            try
            {
                modem.ReceiveTimeount = 1000;
                modem.Connect();
                modem.SendSms(number, text);
            }
            finally
            {
                modem.Close();
            }

            Console.WriteLine("OK");
            Console.ReadLine();
        }
    }
}
