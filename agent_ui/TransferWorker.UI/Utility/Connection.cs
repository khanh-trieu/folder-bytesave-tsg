using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace TransferWorker.UI.Utility
{
   public class Connection
    {
        private string ChuoiKetNoi = @"Server=tcp:bytesavebackup.database.windows.net,1433;Initial Catalog=ByteSave;Persist Security Info=False;User ID=khanh.trieu;Password=Tsg@12345678;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public DataTable LoadData(string sql)
        {
            using (SqlConnection cnn = new SqlConnection(ChuoiKetNoi))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                return dt;
            }
        }
        public DataTable LoadDataParameter(string sql, string[] name, object[] values, int parameter)
        {
            using (SqlConnection cnn = new SqlConnection(ChuoiKetNoi))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameter > 0)
                {
                    for (int i = 0; i < parameter; i++)
                    {
                        cmd.Parameters.AddWithValue(name[i], values[i]);
                    }
                }
                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);
                return dt;
            }
        }
        public bool Execute(string sql, string[] name, object[] values, int parameter)
        {
            using (SqlConnection cnn = new SqlConnection(ChuoiKetNoi))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < parameter; i++)
                {
                    cmd.Parameters.AddWithValue(name[i], values[i]);
                }
                cmd.ExecuteNonQuery();
                return true;
            }
        }
        public object GetOneValue(string sql, string[] name, object[] values, int parameter)
        {
            using (SqlConnection cnn = new SqlConnection(ChuoiKetNoi))
            {
                cnn.Open();
                SqlCommand cmd = new SqlCommand(sql, cnn);
                cmd.CommandType = CommandType.StoredProcedure;
                for (int i = 0; i < parameter; i++)
                {
                    cmd.Parameters.AddWithValue(name[i], values[i]);
                }
                return cmd.ExecuteScalar();
            }
        }
    }
}
