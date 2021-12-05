using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Database
{
  public  interface IBackupService
    {
        void BackupDatabase(string connectionString, string backupFolderFullPath,string googleDriveKey);
    }
}
