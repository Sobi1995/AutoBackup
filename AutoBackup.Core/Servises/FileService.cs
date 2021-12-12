using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Core.Servises
{
    public   class FileService : IFileService
    {
        public   Stream GetFileStream(string fileName) => File.OpenRead(fileName);

        public   string GetFileBaseNameUsingSplit(string path)
        {
            string[] pathArr = path.Split('\\');
            string fileBaseName = pathArr.Last().ToString();
            return fileBaseName;
        }

        public string CreateFolderInCurrent(string folderName)
        {
            string currentPath = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(currentPath, folderName)))
                Directory.CreateDirectory(Path.Combine(currentPath, folderName));
            var fullPatch= $"{currentPath}\\{folderName}";

            DirectoryInfo dInfo = new DirectoryInfo(fullPatch);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);

            return fullPatch;
        }
    }
}
