using Esl2Http.Base;
using Esl2Http.Dal;
using Esl2Http.Delegates;
using Esl2Http.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static Esl2Http.Delegates.EslClientDelegates;

namespace Esl2Http.Parts.HttpPostWorker
{
    class HttpPostWorker : BaseThreadWorker, IHttpPostWorker
    {
        

        #region Args from Start

        EslClientLogDelegate _LogDelegate;
        int? _HttpTimeoutS;
        IDal _dal;

        #endregion

        #region IHttpPostWorker members

        public void Start(
            EslClientLogDelegate LogDelegate,
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
                                            _dal.SetEventAsPosted(EventToPost.Item1, url, (int)resp.StatusCode, resp.ReasonPhrase);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _dal.SetEventAsPosted(EventToPost.Item1, url, null, ex.ToString());
                                }
                            }
                        });
                    }
                }

                httpcli.CancelPendingRequests();
            }
        }


        #endregion

        public void Dispose()
        {
            base.StopWorker();
        }
    }
}
