using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace xQuant.AutoUpdate
{
    /// <summary>
    /// sql ��������
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
        /// Sql �ļ����� ������չ��
        /// </summary>
        public string SqlFileName
        {
            get { return _sqlFileName; }
        }
        /// <summary>
        /// �Ƿ�ɹ�
        /// </summary>
        public bool Success
        {
            get { return _success; }
            private set { _success = value; }
        }

        /// <summary>
        /// ��־�ļ���ַ
        /// </summary>
        public string LogFile
        {
            get { return _logFile; }
            private set { _logFile = value; }
        }
        /// <summary>
        /// ���ݿ������ַ��� 
        /// </summary>
        public string SqlConnectionString
        {
            get { return _sqlConnectionString; }
            set { _sqlConnectionString = value; }
        }
        /// <summary>
        /// ��ȡ�������ṩ������������
        /// </summary>
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        private string _providerName;


        /// <summary>
        /// ִ��SQLPLUS ʧ��ʱ���������Ϣ
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
                        result = string.Format("��{0}���ļ�ִ�д���ԭ��:{1}",this.SqlFile, e.Message);
                        return false;
                    }
                default:
                    result = string.Empty;
                    return false;
            }
        }
        /// <summary>
        /// ����SQLPLUS ���صĽ��
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
