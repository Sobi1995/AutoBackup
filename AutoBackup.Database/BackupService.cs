using AutoBackup.ConsoleApp.Model.Dto;
using AutoBackup.Core.Model;
using AutoBackup.Core.Servises;
using AutoBackup.Core.Servises.Common;
using AutoBackup.Http.GoogleDrive;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AutoBackup.Database
{
    public class BackupService: IBackupService
    {
        private   string _connectionString;
        private   string _backupFolderFullPath;
        private readonly string[] _systemDatabaseNames = { "master", "tempdb", "model", "msdb" };
        public readonly IProgressBar _progressBar;
        public readonly IGoogleDriveHttpService _googleDriveHttpService;

        private readonly IFileService _fileService;
        public BackupService(IProgressBar progressBar, IGoogleDriveHttpService googleDriveHttpService, IFileService fileService)
        {
            _progressBar = progressBar;
            _googleDriveHttpService = googleDriveHttpService;
 
            _fileService = fileService;
        }

        //private void BackupAllUserDatabases()
        //{
        //    foreach (string databaseName in GetAllUserDatabases())
        //    {
        //        //BackupDatabase(databaseName);
        //    }
        //}




        public void BackupDatabase(string connectionString )
        {
           
             var connectionModel=   checkConnectinString(connectionString);
            InitBackupDatabase(connectionString, _fileService.CreateFolderInCurrent($"Temp_{connectionModel.InitialCatalog}"));

            var filePath = BuildBackupPathWithFilename(connectionModel.InitialCatalog);
            try
            {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = String.Format("BACKUP DATABASE [{0}] TO DISK='{1}'", connectionModel.InitialCatalog, filePath.Path+filePath.FileName);
           
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
                  var zipFile= _fileService.Zip(filePath.Path, _backupFolderFullPath, filePath.FolderName);
                    _googleDriveHttpService.UploadDatabse(connectionModel.InitialCatalog, zipFile);
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

        private BuildBackupPathWithFilenameModel BuildBackupPathWithFilename(string databaseName)
        {
            string filename = string.Format("{0}-{1}.bak", databaseName, DateTime.Now.ToString("HH-mm-ss"));
            var folderName= DateTime.Now.ToString("MM-dd-yyyy");
            var path=_fileService.CreateFolderInPath(_backupFolderFullPath, folderName + "\\");
            return new BuildBackupPathWithFilenameModel() { 
            FileName= filename,
            Path=path,
            FolderName= folderName
            };  
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
