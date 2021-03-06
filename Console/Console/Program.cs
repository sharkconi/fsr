﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Data.OleDb;

namespace ConsoleApp
{
    class ConsoleApp
    {
        //string mysql_conn_str = "server=192.168.31.195;User Id=root;password=initial;Database=fsr;CharSet=utf8;";
        //string sqlserver_conn_str = "user id=sa;password=wang419420; initial catalog=banzhaoyun; Server=192.168.3.101\\foodsd;Connect Timeout=30";
        //excel sql_str like: insert into [Sheet1$B1:B1] values('xxx')
        public void mysql_execute_non_query(string conn_str, string sql_str)
        {
            MySqlConnection conn = new MySqlConnection(conn_str);
            MySqlCommand cmd = new MySqlCommand(sql_str, conn);
            cmd.CommandTimeout = 300;

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();

            return;
        }

        public DataTable mysql_execute_get_dt(string conn_str, string sql_str)
        {
            MySqlConnection conn = new MySqlConnection(conn_str);
            MySqlCommand cmd = new MySqlCommand(sql_str, conn);
            cmd.CommandTimeout = 300;

            conn.Open();
            MySqlDataReader dr = cmd.ExecuteReader();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            ds.Tables.Add(dt);
            ds.EnforceConstraints = false;
            dt.Load(dr);
            dr.Close();
            conn.Close();
            conn.Dispose();

            return ds.Tables[0];
        }

        public void sqlserver_execute_non_query(string conn_str, string sql_str)
        {
            SqlConnection sql_conn = new SqlConnection(conn_str); ;
            SqlCommand sql_cmd = new SqlCommand(sql_str);
            sql_cmd.Connection = sql_conn;

            sql_cmd.Connection.Open();
            sql_cmd.ExecuteNonQuery();
            sql_cmd.Connection.Close();
            sql_cmd.Connection.Dispose();
        }

        public DataTable sqlserver_execute_get_dt(string conn_str, string sql_str)
        {
            SqlConnection sql_conn = new SqlConnection(conn_str);
            SqlCommand sql_cmd = new SqlCommand(sql_str);
            DataTable dt = new DataTable();
            sql_cmd.Connection = sql_conn;

            sql_cmd.Connection.Open();
            SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(sql_cmd);
            da.Fill(dt);

            sql_cmd.Connection.Close();
            sql_cmd.Connection.Dispose();

            return dt;
        }

        public DataTable excel_get_dt(string file_path, string sheet_name)
        {
            string connStr = "Provider = Microsoft.ACE.OLEDB.12.0; " + "Data Source = " + file_path + "; " + "; Extended Properties =\"Excel 12.0;HDR=YES;IMEX=1\"";
            DataSet ds = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                conn.Open();
                string sql = "Select * FROM [" + sheet_name + "$]";
                OleDbDataAdapter odda = new OleDbDataAdapter(sql, conn);
                odda.Fill(ds, sheet_name);
                conn.Close();
                conn.Dispose();
            }
            return ds.Tables[0];
        }

        public void excel_execute_non_query(string file_path, string sheet_name, string sql_str)
        {
            string connStr = "Provider = Microsoft.ACE.OLEDB.12.0; " + "Data Source = " + file_path + "; " + "; Extended Properties =\"Excel 12.0;HDR=YES;READONLY=FALSE\"";
            DataSet ds = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(connStr))
            {
                    conn.Open();
                    OleDbCommand cmd = new OleDbCommand(sql_str, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
            }
            return;
        }
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            ConsoleApp console = new ConsoleApp();
        }
    }
}
