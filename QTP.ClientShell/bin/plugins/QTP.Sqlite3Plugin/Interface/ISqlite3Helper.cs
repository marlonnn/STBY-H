using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace QTP.Sqlite3Plugin.Interface
{
    public interface ISqlite3Helper
    {
        /// <summary>
        /// 访问DB，将DATA返回到表格中
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="conn"></param>
        /// <returns></returns>
        DataTable GetReaderSchema(string tableName, SQLiteConnection conn);
        bool IfGetReaderSchema(string tableName, SQLiteConnection conn,string sql);
        
        /// <summary>
        ///  连接DB
        /// </summary>
        /// <param name="dbname"></param>
        /// <param name="ifcreate"></param>
        /// <returns></returns>
        bool ConnectionDb(string dbname, bool ifcreate);
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        bool ExsistTable(string sql, SQLiteConnection conn);

        /// <summary>
        /// update,insert,delete
        /// </summary>
        /// <param name="conn"></param>
        void SqliteExecute(SQLiteConnection conn, string sql);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataTable DbGetTable(SQLiteConnection conn, string sql);


        /// <summary>
        /// 得到某个字段值
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sql"></param>
        /// <param name="tbName"></param>
        /// <param name="filedNo">得到某个字段值</param>
        /// <returns></returns>
        string GetSNValue(SQLiteConnection conn, string sql, string tbName, int filedNo);

        /// <summary>
        /// close DB
        /// </summary>
        /// <param name="dbname"></param>
        void CloseDb(SQLiteConnection con);
    }
}
