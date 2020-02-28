using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SMS_Gate.Model;

namespace SMS_Gate.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly ISender sms;
        private readonly MyDbContext db;
        private readonly ILogger<SmsController> logger;

        public SmsController(ILogger<SmsController> _logger, MyDbContext _db, ISender sender)
        {
            db = _db;
            sms = sender;
            logger = _logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {

                var ls = await db.Clients.OrderByDescending(x => x.id).Take(100).ToListAsync();
                return Ok(ls);

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            try
            {
                var res = sms.Info();
                return Ok(res);

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(SmsModel sms)
        {
            try
            {
                await db.Clients.AddAsync(
                    new Client
                    {
                        phone_num = sms.nomer,
                        text = sms.text,
                        created = DateTime.Now,
                        status = 0
                    }); ;

                await db.SaveChangesAsync();

                return Ok(new { mes = "OK", status = 1 });

            }
            catch (Exception ee)
            {
                return Ok(new { mes = ee.Message + ee.InnerException?.Message, status = 0 });
            }
        }
    }
}
