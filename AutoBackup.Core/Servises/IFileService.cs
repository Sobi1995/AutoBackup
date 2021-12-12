using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Core.Servises
{
   public interface IFileService
    {
        Stream GetFileStream(string fileName);
        string GetFileBaseNameUsingSplit(string path);
        string CreateFolderInCurrent(string folderName);
    }
}
