using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Database
{
  public  interface IBackupService
    {
        public void BackupAllUserDatabases();
        public void BackupDatabase(string databaseName);
        public IEnumerable<string> GetAllUserDatabases();
        public string BuildBackupPathWithFilename(string databaseName);
    }
}
