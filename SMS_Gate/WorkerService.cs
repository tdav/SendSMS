using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SMS_Gate
{
    public class WorkerService :IHostedService, IDisposable
    {
       
        private readonly ISender sms;
        private readonly ILogger logger;
        private Timer timer;

        private readonly int JobIntervalInSecs = 3;
        private struct State
        {
            public static int numberOfActiveJobs = 0;
            public const int maxNumberOfActiveJobs = 1;
        }

        public WorkerService(ILogger<WorkerService> _logger, ISender _sms)
        {
            sms = _sms;
            logger = _logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is starting.");
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(JobIntervalInSecs));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {         
            // Check Run 1 instance
            if (State.numberOfActiveJobs < State.maxNumberOfActiveJobs)
            {
                // Update number of running jobs in one atomic operation. 
                try
                {
                    Interlocked.Increment(ref State.numberOfActiveJobs);
                    logger.LogInformation("Timed Background Service is stopping.");
                    // _timer?.Change(Timeout.Infinite, 0);

                    sms.Run();
                   

                    Console.WriteLine($"Date {DateTime.Now}");
                    logger.LogInformation("Timed Background Service is working.");

                   // _timer?.Change(Timeout.Infinite, 1);
                }
                finally
                {
                    Interlocked.Decrement(ref State.numberOfActiveJobs);
                }
            }
            else
            {
                logger.LogDebug("Job skipped since max number of active processes reached.");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Timed Background Service is stopping.");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        } 
    }
}
