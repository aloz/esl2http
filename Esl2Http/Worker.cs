using Esl2Http.Interfaces;
using Esl2Http.Parts.EslClient;
using Esl2Http.Parts.EslEventQueue;
using Esl2Http.Parts.EslEventQueueDbPersister;
using Esl2Http.Parts.HttpPostWorker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using static Esl2Http.Delegates.EslClientDelegates;

namespace Esl2Http
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        #region Private vars

        IEslClient _eslClient;
        IEslEventQueue _queue;
        IEslEventQueueDbPersister _queuePersister;
        IHttpPostWorker _httpPostWorker;

        #endregion

        #region ctor

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        #endregion

        #region BackgroundService members

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                _queue = new EslEventQueue();

                _eslClient = new EslClient();
                _eslClient.Start(
                    EslClientLogDelegateCallback,
                    EslClientResponseEslEventDelegateCallback,
                    Config.EslEventsToSubscribe,
                    Config.EslRxBufferSize);

                _queuePersister = new EslEventQueueDbPersisterPostgres();
                _queuePersister.Start(
                    EslClientLogDelegateCallback,
                    _queue,
                    Config.DbConnectionString
                    );

                _httpPostWorker = new HttpPostWorker();
                _httpPostWorker.Start(
                    EslClientLogDelegateCallback,
                    Config.HttpTimeoutS,
                    Config.DbConnectionString
                    );
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                CheckThreadsAlive();
                await Task.Delay(1000, stoppingToken);
            }

            WriteToConsoleLog("Exit signal", EslClientLogType.Warning);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            StopAndDisposeParts();
            return base.StopAsync(cancellationToken);
        }

        #endregion

        #region Console logging and delegates

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

        #endregion

        private void CheckThreadsAlive()
        {
            // TODO
        }

        void StopAndDisposeParts()
        {
            // TODO all my IDisposable
            _eslClient.Stop();
            try { _eslClient.Dispose(); } catch { }
            _queuePersister.Stop();
            try { _queuePersister.Dispose(); } catch { }
            try { _queue.Dispose(); } catch { }
            _httpPostWorker.Stop();
            try { _httpPostWorker.Dispose(); } catch { }
        }

        public override void Dispose()
        {
            StopAndDisposeParts();
            base.Dispose();
        }

    }
}