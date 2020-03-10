using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SMS_Gate.Model;
using System;
using System.Linq;

namespace SMS_Gate
{
    public class SmsSender : ISender
    {

        private readonly ILogger<SmsSender> logger;
        private readonly IConfiguration config;
        private DevInfo devInfo;

        public SmsSender(ILogger<SmsSender> _logger, IConfiguration _config)
        {
            logger = _logger;
            config = _config;
        }
         

        public DevInfo Info()
        {
            return devInfo; 
        }

        public void Run()
        {

            using (var db = new MyDbContext())
            {
                var list = db.Clients.Where(x => x.status == 0).ToList();
                if (list.Count == 0) return;

                string ComPort = config.GetSection("AppSettings:ComPort").Value;
                var modem = new GsmModem.GsmModem(ComPort);

                try
                {

                    modem.ReceiveTimeount = 1000;
                    var isOpen = modem.Connect();
                    if (isOpen == false) return;
                    var sig = modem.SignalStrength;
                    logger.LogInformation($"Signal ==> {sig}");

                    if (sig == -1)
                    {
                        logger.LogError($"No signal");
                        return;
                    }


                    if (devInfo == null)
                    {
                        devInfo = new DevInfo()
                        {
                            SignalStrength = modem.SignalStrength,
                            Comands = modem.SupportedCommands.ToList()
                        };
                    }

                    foreach (var it in list)
                    {
                        var res = modem.SendSms(it.phone_num, it.text);

                        if (!res)
                        {
                            logger.LogInformation($"Sended => {it.phone_num}");
                            it.status = 1;
                            it.sended = DateTime.Now;
                        }

                        db.SaveChanges();
                    }

                }
                catch (Exception ee)
                {
                    logger.LogError(ee.Message);
                    logger.LogError(ee.InnerException?.Message);
                    logger.LogError(ee.StackTrace);
                }
                finally
                {
                    modem?.Close();
                }
            }
        }
    }
}
