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
using static Esl2Http.Delegates.Log;

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
        IHttpPostWorker _httpRepostWorker;

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
                    LogDelegateCallback,
                    EslClientResponseEslEventDelegateCallback,
                    Config.EslEventsToSubscribe,
                    Config.EslRxBufferSize);

                _queuePersister = new EslEventQueueDbPersisterPostgres();
                _queuePersister.Start(
                    LogDelegateCallback,
                    _queue,
                    Config.DbConnectionString
                    );

                _httpPostWorker = new HttpPostWorker();
                _httpPostWorker.Start(
                    LogDelegateCallback,
                    Config.HttpTimeoutS,
                    Config.DbConnectionString
                    );

                _httpRepostWorker = new HttpRepostWorker();
                _httpRepostWorker.Start(
                    LogDelegateCallback,
                    Config.HttpTimeoutS,
                    Config.DbConnectionString
                    );
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }

            WriteToConsoleLog("Exit signal", LogType.Warning);
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            StopAndDisposeParts();
            return base.StopAsync(cancellationToken);
        }

        #endregion

        #region Console logging and delegates

        private void WriteToConsoleLog(string str, LogType LogType = LogType.Information)
        {
            string logstr = $"{DateTime.UtcNow}\t{str}";
            if (!logstr.EndsWith("\n"))
                logstr += "\n";
            switch (LogType)
            {
                case LogType.Information:
                    {
                        _logger.LogInformation(logstr);
                        break;
                    }
                case LogType.Warning:
                    {
                        _logger.LogWarning(logstr);
                        break;
                    }
                case LogType.Error:
                    {
                        _logger.LogError(logstr);
                        break;
                    }
                case LogType.Critical:
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

        private void LogDelegateCallback(Type sender, string str, LogType LogType)
        {
            WriteToConsoleLog($"[{sender.Name}] {str}", LogType);
        }

        public void EslClientResponseEslEventDelegateCallback(string str)
        {
            WriteToConsoleLog(str);
            _queue.Enqueue(str);
            WriteToConsoleLog($"{_queue.QueueCount } event(s) in memory Queue");
        }

        #endregion

        void StopAndDisposeParts()
        {
            try { _eslClient.Stop(); } catch { }
            try { _eslClient.Dispose(); } catch { }
            try { _queuePersister.Stop(); } catch { }
            try { _queuePersister.Dispose(); } catch { }
            try { _queue.Dispose(); } catch { }
            try { _httpPostWorker.Stop(); } catch { }
            try { _httpPostWorker.Dispose(); } catch { }
            try { _httpRepostWorker.Stop(); } catch { }
            try { _httpPostWorker.Dispose(); } catch { }
        }

        public override void Dispose()
        {
            StopAndDisposeParts();
            base.Dispose();
        }

    }
}