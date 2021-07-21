using Esl2Http.Interfaces;
using Npgsql;
using System;

namespace Esl2Http.Dal
{
    class DalPostgres : IDal
    {
        string _connectionString;

        protected DalPostgres()
        {
            NpgsqlConnection.GlobalTypeMapper.UseJsonNet();
        }

        public DalPostgres(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }

        public ulong? AddEvent(DateTime arrived, string jsonevent)
        {
            ulong? result = null;

            using (NpgsqlConnection cn = new NpgsqlConnection(_connectionString))
            {
                cn.Open();
                using (NpgsqlCommand cmd = cn.CreateCommand())
                {
                    cmd.CommandText = "call usp_events_add(@arrived, @jsonevent, @out_id)";

                    NpgsqlParameter parm;
                    cmd.Parameters.AddWithValue("arrived", NpgsqlTypes.NpgsqlDbType.Timestamp, arrived);
                    cmd.Parameters.AddWithValue("jsonevent", NpgsqlTypes.NpgsqlDbType.Json, jsonevent);

                    parm = new NpgsqlParameter("out_id", NpgsqlTypes.NpgsqlDbType.Bigint);
                    parm.Direction = System.Data.ParameterDirection.InputOutput;
                    parm.Value = -1;
                    cmd.Parameters.Add(parm);

                    cmd.ExecuteNonQuery();
                    ulong r = 0;
                    if (ulong.TryParse(Convert.ToString(cmd.Parameters["out_id"].Value), out r))
                        result = r;
                }
                cn.Close();
            }

            return result;
        }

        public void Dispose()
        {
            // TODO
        }
    }
}
