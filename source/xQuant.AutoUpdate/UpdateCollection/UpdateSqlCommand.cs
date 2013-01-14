using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// sql 更新命令
    /// </summary>
    public class UpdateSqlCommand
    {
        public string SqlFile;
        public string Command;
        private bool _success;
        private string _sqlFileName;
        public UpdateSqlCommand(string sqlFile, string command, string logFile)
        {
            this.SqlFile = sqlFile;
            this.Command = command;
            this.LogFile = logFile;
            this.Success = false;
            _sqlFileName = new FileInfo(this.SqlFile).Name;
        }

        private string _logFile;
        /// <summary>
        /// Sql 文件名称 包括扩展名
        /// </summary>
        public string SqlFileName
        {
            get { return _sqlFileName; }
        }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get { return _success; }
            private set { _success = value; }
        }

        /// <summary>
        /// 日志文件地址
        /// </summary>
        public string LogFile
        {
            get { return _logFile; }
            private set { _logFile = value; }
        }
        /// <summary>
        /// 数据库连接字符串 
        /// </summary>
        public string SqlConnectionString
        {
            get { return _sqlConnectionString; }
            set { _sqlConnectionString = value; }
        }
        /// <summary>
        /// 获取或设置提供程序名称属性
        /// </summary>
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        private string _providerName;


        /// <summary>
        /// 执行SQLPLUS 失败时输出错误信息
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool ExeCommand(out string result)
        {

            switch (ProviderName)
            {
                case "Oracle.DataAccess.Client":
                    result = Util.ExeCommand(Command);
                    Success = ParseResult(result);
                    return Success;
                case "System.Data.SqlClient":
                    try
                    {
                        result = string.Empty;
                        this.ExecuteSql(this.SqlConnectionString, this.Command);
                        return true;
                    }
                    catch (Exception e)
                    {
                        result = string.Format("【{0}】文件执行错误，原因:{1}",this.SqlFile, e.Message);
                        return false;
                    }
                default:
                    result = string.Empty;
                    return false;
            }
        }
        /// <summary>
        /// 解析SQLPLUS 返回的结果
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool ParseResult(string result)
        {
            if (string.IsNullOrEmpty(result))
            {
                return true;
            }
            return !result.Contains("ORA-");
        }

        private string _sqlConnectionString;

        private void ExecuteSql(string sqlconnection, string commandText)
        {
            using (SqlConnection connection = new SqlConnection(sqlconnection))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.Transaction = transaction;
                            cmd.CommandText = commandText;
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandTimeout = 30;
                            cmd.ExecuteNonQuery();
                            transaction.Commit();
                        }

                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }
    }
}
