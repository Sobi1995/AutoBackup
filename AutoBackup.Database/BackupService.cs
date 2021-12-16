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
        public readonly IMeesageService _meesageService;
      

        private readonly IFileService _fileService;
        public BackupService(
            IProgressBar progressBar, 
            IGoogleDriveHttpService googleDriveHttpService,
            IFileService fileService,
            IMeesageService meesageService)
        {
            _progressBar = progressBar;
            _googleDriveHttpService = googleDriveHttpService;
            _fileService = fileService;
            _meesageService = meesageService;
          _meesageService.Message(@"
    _         _         ____             _                
   / \  _   _| |_ ___  | __ )  __ _  ___| | ___   _ _ __  
  / _ \| | | | __/ _ \ |  _ \ / _` |/ __| |/ / | | | '_ \ 
 / ___ \ |_| | || (_)  | |_) | (_| | (__|   <| |_| | |_) |
/_/   \_\__,_|\__\___/ |____/ \__,_|\___|_|\_\\__,_| .__/ 
                                                  |_|    
",ConsoleColor.Blue);

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
             var connectionModel=checkConnectinString(connectionString);
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
                        //foreach (var element in args.Errors.OfType<SqlError>())
                        //{
                        //    _meesageService.Message($"Class: {element.Class} LineNumber: {element.LineNumber} Message: {element.Message} Number: {element.Number} Procedure: {element.Procedure}",ConsoleColor.Green);
                        //}
                    };
                         connection.Open();
                        _meesageService.Message($"Please wait to Backup {connectionModel.InitialCatalog} database",ConsoleColor.Yellow);
                         int result= command.ExecuteNonQuery();
                        _meesageService.Message($"success .the addrress od dabatabse backup is  {filePath.Path + filePath.FileName} ",ConsoleColor.Green);

                        _meesageService.Message($"Please wait to zip {connectionModel.InitialCatalog} database", ConsoleColor.Yellow);
                        var zipFile= _fileService.Zip(filePath.Path, _backupFolderFullPath, filePath.FolderName);
                        _fileService.DeleteFolder(filePath.Path);
                        _meesageService.Message($"success .the addrress of zip dabatabse   is  {_backupFolderFullPath} ", ConsoleColor.Green);
                        _googleDriveHttpService.UploadDatabse(connectionModel.InitialCatalog, zipFile);
                }
            }
            }
            catch (Exception ex)
            {

                _meesageService.Message(ex.Message,ConsoleColor.Red);
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
            var folderName= DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss");
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
