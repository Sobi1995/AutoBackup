using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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

        public string GetFileBaseJustName(string path)
        {
            string[] pathArr = path.Split('\\');
            string[] fileArr = pathArr.Last().Split('.');
            return fileArr.First().ToString();
       
        }

        public string CreateFolderInCurrent(string folderName)
        {
          
            string currentPath = Directory.GetCurrentDirectory();
            if (!Directory.Exists(Path.Combine(currentPath, folderName)))
                Directory.CreateDirectory(Path.Combine(currentPath, folderName));
            var fullPatch= $"{currentPath}\\{folderName}";
            AllowPermission(fullPatch);
            return fullPatch;
        }

        public string Zip(string source, string destination,string fileName)
        {
    
            ZipFile.CreateFromDirectory(source, $"{destination}\\{GetFileBaseJustName(fileName)}.zip", CompressionLevel.Optimal,false);
            return $"{destination}\\{GetFileBaseJustName(fileName)}.zip";
        }

        public string CreateFolderInPath(string source, string folderName)
        {
          
            if (!Directory.Exists(Path.Combine(source, folderName)))
                Directory.CreateDirectory(Path.Combine(source, folderName));
            var fullPatch = $"{source}\\{folderName}";
            AllowPermission(fullPatch);
            return fullPatch;
        }

        private void AllowPermission(string Patch)
        {
            DirectoryInfo dInfo = new DirectoryInfo(Patch);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
        }

        public void DeleteFolder(string sourse)
        {
      
           System.IO.Directory.Delete(sourse, true);
        }

        public void DeleteFile(string sourse)
        {
            System.IO.File.Delete(sourse);
        }
    }
}
