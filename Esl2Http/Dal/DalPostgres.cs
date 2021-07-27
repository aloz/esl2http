using Esl2Http.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;

namespace Esl2Http.Dal
{
    class DalPostgres : IDal
    {
        string _connectionString;

        protected DalPostgres()
        {
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
        }

        public DalPostgres(string ConnectionString) : this()
        {
            _connectionString = ConnectionString;
        }

        public long? AddNewEvent(DateTime arrived, string jsonevent)
        {
            long? result = null;

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "call usp_events_add(@arrived, @jsonevent, @out_id);";

                    cmd.Parameters.AddWithValue("arrived", NpgsqlTypes.NpgsqlDbType.Timestamp, arrived);
                    cmd.Parameters.AddWithValue("jsonevent", NpgsqlTypes.NpgsqlDbType.Json, jsonevent);

                    NpgsqlParameter parm;
                    parm = new NpgsqlParameter("out_id", NpgsqlTypes.NpgsqlDbType.Bigint);
                    parm.Direction = System.Data.ParameterDirection.InputOutput;
                    parm.Value = 0;
                    cmd.Parameters.Add(parm);

                    cmd.ExecuteNonQuery();
                    long r = 0;
                    if (long.TryParse(Convert.ToString(cmd.Parameters["out_id"].Value), out r))
                        result = r;
                }
                cn.Close();
            }

            return result;
        }
        public string[] GetHttpHandlers()
        {
            string[] result = null;
            List<string> lstResult = new List<string>();

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "select * from fn_get_http_handlers();";
                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lstResult.Add(Convert.ToString(rdr["url"]));
                        }
                        rdr.Close();
                        result = lstResult.ToArray();
                    }
                }
                cn.Close();
            }

            return result;
        }

        public string[] GetHttpHandlersToRepost()
        {
            string[] result = null;
            List<string> lstResult = new List<string>();

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "select * from fn_get_http_handlers_torepost();";
                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            lstResult.Add(Convert.ToString(rdr["url"]));
                        }
                        rdr.Close();
                        result = lstResult.ToArray();
                    }
                }
                cn.Close();
            }

            return result;
        }

        public Tuple<int?> GetConfig()
        {
            Tuple<int?> result;

            int? timeout_s_http = null;

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "select * from fn_get_config();";
                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            object obj_timeout_s_http = rdr["timeout_s_http"];
                            if (obj_timeout_s_http != DBNull.Value)
                                timeout_s_http = Convert.ToInt32(obj_timeout_s_http);
                        }
                        rdr.Close();
                    }
                }
            }

            result = new Tuple<int?>(timeout_s_http);
            return result;
        }

        public List<Tuple<long, string>> GetEventsToPost(string url)
        {
            List<Tuple<long, string>> result = new List<Tuple<long, string>>();

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "select * from fn_get_events_topost(@url);";
                    cmd.Parameters.AddWithValue("url", NpgsqlTypes.NpgsqlDbType.Text, url);
                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            result.Add(Tuple.Create<long, string>((long)rdr["event_id"], (string)rdr["event_jsonb"]));
                        }
                        rdr.Close();
                    }
                }
            }

            return result;
        }

        public List<Tuple<long, string>> GetEventsToRepost(string url)
        {
            List<Tuple<long, string>> result = new List<Tuple<long, string>>();

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "select * from fn_get_events_torepost(@url);";
                    cmd.Parameters.AddWithValue("url", NpgsqlTypes.NpgsqlDbType.Text, url);
                    using (NpgsqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            result.Add(Tuple.Create<long, string>((long)rdr["event_id"], (string)rdr["event_jsonb"]));
                        }
                        rdr.Close();
                    }
                }
            }

            return result;
        }

        public long? SetEventAsPosted(long event_id, string url, int? statuscode, string reason_phrase)
        {
            long? result = null;

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "call usp_events_set_as_posted(@event_id, @url, @statuscode, @reason_phrase, @out_id);";

                    cmd.Parameters.AddWithValue("event_id", NpgsqlTypes.NpgsqlDbType.Bigint, event_id);
                    cmd.Parameters.AddWithValue("url", NpgsqlTypes.NpgsqlDbType.Text, url);

                    if (statuscode.HasValue)
                        cmd.Parameters.AddWithValue("statuscode", NpgsqlTypes.NpgsqlDbType.Integer, statuscode);
                    else
                        cmd.Parameters.AddWithValue("statuscode", NpgsqlTypes.NpgsqlDbType.Integer, DBNull.Value);

                    cmd.Parameters.AddWithValue("reason_phrase", NpgsqlTypes.NpgsqlDbType.Text, reason_phrase);

                    NpgsqlParameter parm;
                    parm = new NpgsqlParameter("out_id", NpgsqlTypes.NpgsqlDbType.Bigint);
                    parm.Direction = System.Data.ParameterDirection.InputOutput;
                    parm.Value = 0;
                    cmd.Parameters.Add(parm);

                    cmd.ExecuteNonQuery();
                    long r = 0;
                    if (long.TryParse(Convert.ToString(cmd.Parameters["out_id"].Value), out r))
                        result = r;
                }
                cn.Close();
            }

            return result;
        }

        public bool? IsResendAvailable(string url)
        {
            bool? result = null;

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "call usp_is_resend_available(@url, @out_is_available);";

                    cmd.Parameters.AddWithValue("url", NpgsqlTypes.NpgsqlDbType.Text, url);

                    NpgsqlParameter parm;
                    parm = new NpgsqlParameter("out_is_available", NpgsqlTypes.NpgsqlDbType.Boolean);
                    parm.Direction = System.Data.ParameterDirection.InputOutput;
                    parm.Value = false;
                    cmd.Parameters.Add(parm);

                    cmd.ExecuteNonQuery();
                    bool r = false;
                    if (bool.TryParse(Convert.ToString(cmd.Parameters["out_is_available"].Value), out r))
                        result = r;
                }
                cn.Close();
            }

            return result;
        }

        public void Dispose()
        {

        }
    }
}