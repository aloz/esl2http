using Esl2Http.Common.Interfaces;
using Esl2Http.Esl;
using Esl2Http.Queue;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Esl2Http.Common.EslClientDelegates;

namespace Esl2Http
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private IEslClient _eslClient;
        private IQueue _queue;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                _queue = new Queue4Events();

                _eslClient = new EslClient();
                _eslClient.Start(EslClientLogDelegateCallback, EslClientResponseEslEventDelegateCallback,
                    Config.EslEventsToSubscribe,
                    Config.EslRxBufferSize);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                CheckThreadsAlive();
                await Task.Delay(1000, stoppingToken);
            }

            WriteToConsoleLog("Exit signal", EslClientLogType.Warning);
            _eslClient.Stop();
            try
            {
                _eslClient.Dispose();
            }
            catch { }
        }

        private void WriteToConsoleLog(string str, EslClientLogType LogType = EslClientLogType.Information)
        {
            string logstr = $"{DateTime.UtcNow}\t{str}";
            if (!logstr.EndsWith("\n"))
                logstr += "\n";
            switch (LogType)
            {
                case EslClientLogType.Information:
                    {
                        _logger.LogInformation(logstr);
                        break;
                    }
                case EslClientLogType.Warning:
                    {
                        _logger.LogWarning(logstr);
                        break;
                    }
                case EslClientLogType.Error:
                    {
                        _logger.LogError(logstr);
                        break;
                    }
                case EslClientLogType.Critical:
                    {
                        _logger.LogCritical(logstr);
                        break;
                    }
                default:
                    {
                        _logger.LogInformation(logstr);
                        break;
                    }
            }
        }

        private void EslClientLogDelegateCallback(string str, EslClientLogType LogType)
        {
            WriteToConsoleLog(str, LogType);
        }

        public void EslClientResponseEslEventDelegateCallback(string str)
        {
            WriteToConsoleLog(str);
            _queue.Enqueue(str);
            WriteToConsoleLog($"{_queue.QueueCount } event(s) in Queue");
        }

        private void CheckThreadsAlive()
        {
            // TODO
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            // TODO all my IDisposable
            try { _eslClient.Dispose(); } catch { }
            try { _queue.Dispose(); } catch { }
            return base.StopAsync(cancellationToken);
        }

    }
}
