using AutoBackup.ConsoleApp.Model.Dto;
using AutoBackup.Core.Servises.Common;
using AutoBackup.Http.GoogleDrive;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace AutoBackup.Database
{
    public class BackupService: IBackupService
    {
        private   string _connectionString;
        private   string _backupFolderFullPath;
        private readonly string[] _systemDatabaseNames = { "master", "tempdb", "model", "msdb" };
        public readonly IProgressBar _progressBar;
        public readonly IGoogleDriveHttpService _googleDriveHttpService;
        public BackupService(IProgressBar progressBar, IGoogleDriveHttpService googleDriveHttpService)
        {
            _progressBar = progressBar;
            _googleDriveHttpService = googleDriveHttpService;
        }

        //private void BackupAllUserDatabases()
        //{
        //    foreach (string databaseName in GetAllUserDatabases())
        //    {
        //        //BackupDatabase(databaseName);
        //    }
        //}




        public void BackupDatabase(string connectionString, string backupFolderFullPath, string googleDriveKey)
        {
            InitBackupDatabase(connectionString, backupFolderFullPath);
             var connectionModel=   checkConnectinString(connectionString);
            string filePath = BuildBackupPathWithFilename(connectionModel.InitialCatalog);
            try
            {

           
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", connectionModel.InitialCatalog, filePath.Trim());
           
                using (var command = new SqlCommand(query, connection))
                {
                    connection.InfoMessage += (sender, args) =>
                    {
                        foreach (var element in args.Errors.OfType<SqlError>())
                        {
                            Console.WriteLine($"Class: {element.Class} LineNumber: {element.LineNumber} Message: {element.Message} Number: {element.Number} Procedure: {element.Procedure}");
                        }
                    };
                    connection.Open();
                   int result= command.ExecuteNonQuery();
                   _googleDriveHttpService.UploadDatabse(connectionModel.InitialCatalog, filePath.Trim());
                }
            }
            }
            catch (Exception ex)
            {

                throw new System.ArgumentException(ex.Message);
            }
        }

        private IEnumerable<string> GetAllUserDatabases()
        {
            var databases = new List<String>();

            DataTable databasesTable;

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                databasesTable = connection.GetSchema("Databases");

                connection.Close();
            }

            foreach (DataRow row in databasesTable.Rows)
            {
                string databaseName = row["database_name"].ToString();

                if (_systemDatabaseNames.Contains(databaseName))
                    continue;

                databases.Add(databaseName);
            }

            return databases;
        }

        private string BuildBackupPathWithFilename(string databaseName)
        {
            string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("yyyy-MM-dd"));

            return Path.Combine(_backupFolderFullPath, filename);
        }



        private void InitBackupDatabase(string connectionString, string backupFolderFullPath)
        {
            _connectionString = connectionString;
            _backupFolderFullPath = backupFolderFullPath;
        }


        private ConnectionDetilesModel checkConnectinString(string connectionString)
        {

            try
            {
                var connectionModel = GetConnectionDetiles(connectionString);
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand($"SELECT db_id('{connectionModel.InitialCatalog}')", connection))
                    {
                        connection.Open();
                        var isConnect = (command.ExecuteScalar() != DBNull.Value);
                        Console.WriteLine("Connection is valid ");
                        return GetConnectionDetiles(connectionString);
                    }
                }
            }
            catch (Exception ex)
            {

                throw new System.ArgumentException($"Connection not Valid {ex.Message}");
            }

        }

        private ConnectionDetilesModel GetConnectionDetiles(string connection)
        {

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connection);
            return new ConnectionDetilesModel()
            {
                DataSource = builder.DataSource,
                InitialCatalog = builder.InitialCatalog,
                Password = builder.Password,
                UserID = builder.UserID

            };
        }
    }
}
