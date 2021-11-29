using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Database
{
  public  interface IBackupService
    {
          void BackupAllUserDatabases();
          IEnumerable<string> GetAllUserDatabases();
          string BuildBackupPathWithFilename(string databaseName);
          void InitBackupDatabase(string connectionString, string backupFolderFullPath);
         void BackupDatabase(string connectionString);

    }
}
