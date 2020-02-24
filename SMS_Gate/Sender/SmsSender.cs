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


        public SmsSender(ILogger<SmsSender> _logger, IConfiguration _config)
        {
            logger = _logger;
            config = _config;
        }


        public void Run()
        {

            using (var db = new MyContext())
            {
                var list = db.Clients.Where(x => x.status == 0 && x.fail_count <= 3).ToList();

                if (list == null || list.Count == 0) return;

                string ComPort = config.GetSection("AppSettings:ComPort").Value;

                var modem = new GsmModem.GsmModem(ComPort);
                try
                {
                    modem.ReceiveTimeount = 1000;
                    modem.ConnectAsync().GetAwaiter().GetResult();

                    foreach (var it in list)
                    {
                        var res = modem.SendSmsAsync(it.phone_num, it.text).GetAwaiter().GetResult();

                        if (!res)
                        {
                            logger.LogInformation($"Sended => {it.phone_num}");

                            it.status = 1;
                        }
                        else
                        {
                            logger.LogError($"Send fail => {it.phone_num}");
                            it.fail_count++;
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
                    modem.Close();
                }
            }
        }
    }
}
