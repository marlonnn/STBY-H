using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QTP.Sqlite3Plugin.Interface;
using System.Data;
using System.Data.SQLite;

namespace QTP.Sqlite3Plugin.Implement
{
    public class Sqlite3Helper:ISqlite3Helper
    {
        public SQLiteConnection conn;
        public SQLiteCommand cmd;

        public System.Data.DataTable GetReaderSchema(string tableName, System.Data.SQLite.SQLiteConnection conn)
        {
            DataTable schemaTable = null;
            cmd = new SQLiteCommand();
            cmd.CommandText = string.Format("select * from [{0}]", tableName);
            cmd.Connection = conn;
            using (IDataReader reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly))
            {
                schemaTable = reader.GetSchemaTable();
            }
            return schemaTable;
        }

        public bool IfGetReaderSchema(string tableName, System.Data.SQLite.SQLiteConnection conn, string sql)
        {
            cmd = new SQLiteCommand();
            cmd.CommandText = sql;
            cmd.Connection = conn;
            SQLiteDataReader reader = cmd.ExecuteReader();

            if (!reader.HasRows)
            {
                if (!reader.IsClosed)
                {
                    reader.Close();
                    reader.Dispose();
                }
                return false;
            }
            else
            {
                if (!reader.IsClosed)
                {
                    reader.Close();
                    reader.Dispose();
                }
                return true;
            }
        }

        public bool ConnectionDb(string dbname, bool ifcreate)
        {
            if (conn != null)
            {
                return false;
            }
            if (ifcreate == true)
            {
                SQLiteConnection.CreateFile(dbname);
            }

            conn = new SQLiteConnection();
            cmd = new SQLiteCommand();
            SQLiteConnectionStringBuilder connstr = new SQLiteConnectionStringBuilder();
            connstr.DataSource = dbname;
            connstr.Password = "casco123";
            conn.ConnectionString = connstr.ToString();
            conn.Open();
            return true;
        }

        public bool ExsistTable(string sql, System.Data.SQLite.SQLiteConnection conn)
        {
            cmd = new SQLiteCommand();
            cmd.CommandText = sql;
            cmd.Connection = conn;
            object obj = cmd.ExecuteScalar();
            if (!Convert.IsDBNull(obj))
            {
                if (obj == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public void SqliteExecute(System.Data.SQLite.SQLiteConnection conn, string sql)
        {
            cmd.CommandText = sql;
            cmd.Connection = conn;
            cmd.ExecuteNonQuery();
        }

        public System.Data.DataTable DbGetTable(System.Data.SQLite.SQLiteConnection conn, string sql)
        {
            SQLiteDataAdapter da = new SQLiteDataAdapter(sql, conn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        public string GetSNValue(System.Data.SQLite.SQLiteConnection conn, string sql, string tbName, int filedNo)
        {
            string filedValue = "";
            SQLiteDataReader reader = GetReader(tbName, conn, sql);
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    filedValue = reader.GetString(filedNo);
                }
            }
            reader.Close();
            return filedValue;
        }

        public SQLiteDataReader GetReader(string tableName, SQLiteConnection conn, string sql)
        {
            cmd = new SQLiteCommand();
            cmd.CommandText = sql;
            cmd.Connection = conn;
            SQLiteDataReader reader = cmd.ExecuteReader();
            return reader;
        }

        public void CloseDb(System.Data.SQLite.SQLiteConnection con)
        {
            con.Close();
        }
    }
}
