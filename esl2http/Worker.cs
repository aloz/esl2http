using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace esl2http
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new Exception($"{DateTime.Now} Crashed! :)))");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Information - Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogWarning("Warning - Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogError("Error - Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogCritical("Critical - Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
