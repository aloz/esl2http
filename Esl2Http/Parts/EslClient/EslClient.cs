using Esl2Http.Base;
using Esl2Http.Interfaces;
using Esl2Http.Private;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Sockets;
using System.Text;
using static Esl2Http.Delegates.EslClientDelegates;
using static Esl2Http.Delegates.Log;

namespace Esl2Http.Parts.EslClient
{
    class EslClient : BaseThreadWorker, IEslClient
    {
        #region const

        const string CONST_CMD_AUTH = "auth";
        const string CONST_CMD_EVENTS_SUBSCRIBE = "event json";
        const string CONST_CMD_BYE = "exit";

        const int CONST_PORT_DEFAULT = 8021;
        const string CONST_EVENT_DEFAULT = "HEARTBEAT";

        static readonly char CONST_SEPARATOR_COLON = ":"[0];
        static readonly char CONST_SEPARATOR_NEWLINE = "\n"[0];

        const string CONST_ESLH_ContentType_NAME = "Content-Type";
        const string CONST_ESLH_ContentType_VALUE_AuthRequest = "auth/request";
        const string CONST_ESLH_ContentType_VALUE_CommandReply = "command/reply";
        const string CONST_ESLH_ContentType_VALUE_EventJson = "text/event-json";
        const string CONST_ESLH_ContentType_VALUE_DisconnectedNotice = "text/disconnect-notice";

        const string CONST_ESLH_ReplyText_NAME = "Reply-Text";
        const string CONST_ESLH_ReplyText_VALUE_ReplyTextOk_STARTSWITH = "+OK";

        const string CONST_ESLH_ContentLength_NAME = "Content-Length";

        #endregion

        #region Args from Start

        LogDelegate _LogDelegate;
        EslClientResponseEslEventDelegate _ResponseEventDelegate;
        string _EslEventsList;
        int _RxBufferSize;

        #endregion

        #region IEslClient members

        public void Start(
            LogDelegate LogDelegate,
            EslClientResponseEslEventDelegate ResponseEventDelegate,
            string EslEventsList,
            int RxBufferSize)
        {
            _LogDelegate = LogDelegate;
            _ResponseEventDelegate = ResponseEventDelegate;
            _RxBufferSize = RxBufferSize;

            if (!string.IsNullOrEmpty(EslEventsList))
            {
                _EslEventsList = EslEventsList.Replace(CONST_EVENT_DEFAULT, string.Empty);
                _EslEventsList = $"{CONST_EVENT_DEFAULT} {_EslEventsList}";
            }
            else
                _EslEventsList = CONST_EVENT_DEFAULT;

            base.StartWorker();
        }

        public void Stop()
        {
            base.StopWorker();
        }

        #endregion

        #region Overrides

        protected override void Worker()
        {

            byte[] buffRx = new byte[_RxBufferSize];

            StringDictionary dictHeaders = new StringDictionary();
            int headersCount = -1;

            bool IsStreamHeaderReceivedContentTypeEventJson = false;
            int StreamEventContentLen = 0;

            using (Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                if (!IsEx)
                {
                    StartSocketConnection();
                    sock.ReceiveBufferSize = buffRx.Length;

                    headersCount = GetCommandReply(null, CONST_ESLH_ContentType_NAME);

                    if (!(
                        headersCount == 1
                        && dictHeaders[CONST_ESLH_ContentType_NAME] == CONST_ESLH_ContentType_VALUE_AuthRequest
                        ))
                        throw new InvalidOperationException("auth expected");


                    //////////////////////////////////////////////////////////////////////


                    headersCount = GetCommandReply($"{CONST_CMD_AUTH} {Secrets.CONST_SECRETS_EslPassword}",
                        CONST_ESLH_ContentType_NAME,
                        CONST_ESLH_ReplyText_NAME);

                    if (!(
                        headersCount == 2
                        && dictHeaders[CONST_ESLH_ContentType_NAME] == CONST_ESLH_ContentType_VALUE_CommandReply
                        && dictHeaders[CONST_ESLH_ReplyText_NAME].StartsWith(CONST_ESLH_ReplyText_VALUE_ReplyTextOk_STARTSWITH)
                        ))
                        throw new InvalidOperationException("wrong esl password");

                    //////////////////////////////////////////////////////////////////////

                    headersCount = GetCommandReply($"{CONST_CMD_EVENTS_SUBSCRIBE} {_EslEventsList}",
                        CONST_ESLH_ContentType_NAME,
                        CONST_ESLH_ReplyText_NAME);

                    if (!(
                        headersCount == 2
                        && dictHeaders[CONST_ESLH_ContentType_NAME] == CONST_ESLH_ContentType_VALUE_CommandReply
                        && dictHeaders[CONST_ESLH_ReplyText_NAME].StartsWith(CONST_ESLH_ReplyText_VALUE_ReplyTextOk_STARTSWITH)
                        ))
                        throw new InvalidOperationException("wrong esl subscription event(s) name");

                    //////////////////////////////////////////////////////////////////////

                    if (IsEx)
                        SayByeBye();
                }
                while (!IsEx)
                {
                    // Receive events
                    // the stream is without data integrity

                    StringBuilder sb = new StringBuilder();

                    string buffString;
                    do
                    {
                        int rclen = sock.Receive(buffRx);
                        buffString = ASCIIEncoding.ASCII.GetString(buffRx, 0, rclen);
                        sb.Append(buffString);
                    } while (sock.Available > 0 || buffString.LastIndexOf("}"[0]) != buffString.Length - 1);

                    string[] buffStringSplit = sb.ToString()
                        .Replace(CONST_ESLH_ContentLength_NAME, $"\n{CONST_ESLH_ContentLength_NAME}")
                        .Replace(CONST_ESLH_ContentType_NAME, $"\n{CONST_ESLH_ContentType_NAME}")
                        .Split(CONST_SEPARATOR_NEWLINE, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string str in buffStringSplit)
                    {
                        if (
                            IsStreamHeaderReceivedContentTypeEventJson
                            && StreamEventContentLen > 0)
                        {
                            if (str.Length == StreamEventContentLen)
                            {
                                _ResponseEventDelegate(str);
                            }
                            else
                            {
                                _LogDelegate(this.GetType(), $"Event is malformed!", LogType.Error);
                                _LogDelegate(this.GetType(), $"Try to increase EslRxBufferSize and check metwork latency and the server performance", LogType.Error);
                                _LogDelegate(this.GetType(), $"Event received: {str}", LogType.Error);
                                _LogDelegate(this.GetType(), str, LogType.Error);
                            }
                            IsStreamHeaderReceivedContentTypeEventJson = false;
                            StreamEventContentLen = 0;
                        }
                        else
                        {
                            Tuple<string, string> hkv = SplitHeaderKeyValue(str);
                            if (
                                hkv.Item1 == CONST_ESLH_ContentType_NAME
                                && hkv.Item2 == CONST_ESLH_ContentType_VALUE_EventJson
                                )
                            {
                                IsStreamHeaderReceivedContentTypeEventJson = true;
                            }
                            else if (
                                hkv.Item1 == CONST_ESLH_ContentLength_NAME)
                            {
                                int.TryParse(hkv.Item2, out StreamEventContentLen);
                            }
                        }
                    }
                }
                SayByeBye();
                try { sock.Close(); } catch { }

                void StartSocketConnection()
                {
                    string host = null;
                    int port = -1;
                    ObtainHostPort(out host, out port);

                    if (string.IsNullOrEmpty(host) || port == -1)
                        throw new ArgumentException("Check EslHostPort");

                    sock.Connect(host, port);
                }

                void ObtainHostPort(out string host, out int port)
                {
                    host = null;
                    port = -1;

                    string[] eslhostport = Secrets.CONST_SECRETS_EslHostPort.Split(CONST_SEPARATOR_COLON,
                        StringSplitOptions.RemoveEmptyEntries);

                    if (eslhostport.Length >= 1)
                        host = eslhostport[0];

                    switch (eslhostport.Length)
                    {
                        case 1:
                            {
                                port = CONST_PORT_DEFAULT;
                                break;
                            }
                        case 2:
                            {
                                int.TryParse(eslhostport[1], out port);
                                break;
                            }
                    }
                }

                void SayByeBye()
                {
                    headersCount = GetCommandReply(CONST_CMD_BYE,
                        CONST_ESLH_ContentType_NAME,
                        CONST_ESLH_ReplyText_NAME);

                    /*
                    if (!(
                        headersCount == 2
                        && dictHeaders[CONST_ESLH_ContentType_NAME] == CONST_ESLH_ContentType_VALUE_CommandReply
                        && dictHeaders[CONST_ESLH_ReplyText_NAME].StartsWith(CONST_ESLH_ReplyText_VALUE_ReplyTextOk_STARTSWITH)
                        ))
                            throw new InvalidOperationException("wrong exit command");
                    */

                    // Receive disconnected notice
                    headersCount = GetCommandReply(null,
                        CONST_ESLH_ContentType_NAME,
                        CONST_ESLH_ContentLength_NAME);

                    int contentLen = -1;
                    if (
                        headersCount == 2
                        && dictHeaders[CONST_ESLH_ContentType_NAME] == CONST_ESLH_ContentType_VALUE_DisconnectedNotice
                        && int.TryParse(dictHeaders[CONST_ESLH_ContentLength_NAME], out contentLen)
                        )
                    {
                        int rclen = sock.Receive(buffRx, contentLen, SocketFlags.None);
                        string buffString = ASCIIEncoding.ASCII.GetString(buffRx, 0, rclen);
                        _LogDelegate(this.GetType(), buffString);
                    }
                    /*
                    else
                        _LogDelegate("Something wrong with Disconnected Notice");
                    */
                }

                int GetCommandReply(string command, params string[] headernames)
                {
                    int result;

                    if (!string.IsNullOrEmpty(command))
                    {
                        byte[] buffTx = ASCIIEncoding.ASCII.GetBytes($"{command}\n\n");
                        if (!command.StartsWith(CONST_CMD_AUTH))
                            _LogDelegate(this.GetType(), command);
                        sock.Send(buffTx);
                    }

                    int rclen = sock.Receive(buffRx);
                    string buffString = ASCIIEncoding.ASCII.GetString(buffRx, 0, rclen);
                    _LogDelegate(this.GetType(), buffString);

                    dictHeaders.Clear();
                    foreach (string headername in headernames)
                    {
                        dictHeaders.Add(headername, null);
                    }
                    result = GetKnownHeadersValue(buffString, ref dictHeaders);
                    return result;
                }
            }

            int GetKnownHeadersValue(string headers, ref StringDictionary dictKnownHeaders)
            {
                int result = 0;
                String[] hdrs = headers.Split(CONST_SEPARATOR_NEWLINE, StringSplitOptions.RemoveEmptyEntries);
                foreach (string hdr in hdrs)
                {
                    Tuple<string, string> hkv = SplitHeaderKeyValue(hdr);
                    if (hkv != null)
                    {
                        string key = hkv.Item1;
                        string val = hkv.Item2;
                        if (dictKnownHeaders.ContainsKey(key))
                        {
                            dictKnownHeaders[key] = val;
                            result++;
                        }
                    }
                }
                return result;
            }

            Tuple<string, string> SplitHeaderKeyValue(string header)
            {
                Tuple<string, string> result = null;
                List<string> lst = new List<string>();
                string[] split = header.Split(CONST_SEPARATOR_COLON);
                if (split.Length == 2)
                    result = new Tuple<string, string>(split[0].Trim(), split[1].Trim());
                return result;
            }

        }

        #endregion

        public void Dispose()
        {
            base.StopWorker();
        }
    }
}