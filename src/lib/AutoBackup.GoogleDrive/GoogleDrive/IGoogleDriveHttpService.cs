﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup.Http.GoogleDrive
{
  public  interface IGoogleDriveHttpService
    {
        void UploadDatabse(string fileName,string patch);
    }
}
