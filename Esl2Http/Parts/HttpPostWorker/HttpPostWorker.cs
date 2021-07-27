using Esl2Http.Base;
using Esl2Http.Dal;
using Esl2Http.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Esl2Http.Delegates.Log;

namespace Esl2Http.Parts.HttpPostWorker
{
    class HttpPostWorker : BaseThreadWorker, IHttpPostWorker
    {
        #region Args from Start

        LogDelegate _LogDelegate;
        int? _HttpTimeoutS;
        IDal _dal;

        #endregion

        #region IHttpPostWorker members

        public void Start(
            LogDelegate LogDelegate,
            int? HttpTimeoutS,
            string ConnectionString)
        {
            _LogDelegate = LogDelegate;
            _HttpTimeoutS = HttpTimeoutS;
            _dal = new DalPostgres(ConnectionString);
            base.StartWorker();
        }

        public void Stop()
        {
            base.StopWorker();
        }

        #endregion

        #region BaseThreadWorker members

        protected override void Worker()
        {
            using (HttpClient httpcli = new HttpClient())
            {
                if (!_HttpTimeoutS.HasValue)
                    httpcli.Timeout = TimeSpan.FromMilliseconds(-1); // infinite timeout, NOT RECOMMENDED!
                else
                    httpcli.Timeout = TimeSpan.FromSeconds(_HttpTimeoutS.Value);

                while (!IsEx)
                {
                    try
                    {
                        string[] urls = _dal.GetHttpHandlers();
                        if (urls.Length > 0)
                        {
                            Parallel.ForEach(urls, url =>
                            {
                                List<Tuple<long, string>> EventsToPost = _dal.GetEventsToPost(url);
                                foreach (Tuple<long, string> EventToPost in EventsToPost)
                                {
                                    try
                                    {
                                        using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, url))
                                        {
                                            // Header HTTP_HEADER_IS_RESEND_AVAILABLE is not used according we stop to post to handler after an error
                                            // but this is designed
                                            /*
                                            int iIsResentAvailable = 0;
                                            bool? isResentAvailable = _dal.IsResendAvailable(url);
                                            if (!isResentAvailable.HasValue)
                                                iIsResentAvailable = 0;
                                            else
                                                iIsResentAvailable = isResentAvailable.Value ? 1 : 0;
                                            */

                                            req.Headers.Add($"{Const.CONST_HTTP_HEADER_STARTSWITH}{Const.CONST_HTTP_HEADER_THIS_IS_RESEND}", "0");
                                            //req.Headers.Add($"{Const.CONST_HTTP_HEADER_STARTSWITH}{Const.CONST_HTTP_HEADER_IS_RESEND_AVAILABLE}", Convert.ToString(iIsResentAvailable));

                                            // A hack to avoid Internal Server Error on the test environment.
                                            // Nor for production.
                                            // Just to avoid remote settings of the max. request length.
                                            // A hard hack :)
                                            req.Content = new StringContent(
                                                req.RequestUri.Host == "ptsv2.com" ? EventToPost.Item2.Substring(0, 1500) : EventToPost.Item2,
                                                Encoding.UTF8, "application/json");

                                            using (HttpResponseMessage resp = httpcli.Send(req))
                                            {
                                                _LogDelegate(
                                                    this.GetType(),
                                                    $"{(int)resp.StatusCode} {resp.ReasonPhrase} {url}",
                                                    resp.IsSuccessStatusCode ? LogType.Information : LogType.Warning);

                                                _dal.SetEventAsPosted(EventToPost.Item1, url, (int)resp.StatusCode, resp.ReasonPhrase);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _LogDelegate(this.GetType(), ex.ToString(), LogType.Error);
                                        _dal.SetEventAsPosted(EventToPost.Item1, url, null, ex.ToString());
                                        System.Threading.Thread.Sleep(1000);
                                    }
                                }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _LogDelegate(this.GetType(), ex.ToString(), LogType.Error);
                        System.Threading.Thread.Sleep(1000);
                    }

                }
                try { httpcli.CancelPendingRequests(); } catch { }
            }
        }

        #endregion

        public void Dispose()
        {
            base.StopWorker();
        }
    }
}