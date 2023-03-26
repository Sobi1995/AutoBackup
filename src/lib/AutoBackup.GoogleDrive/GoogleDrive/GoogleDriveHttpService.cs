using AutoBackup.Core;
using AutoBackup.Core.Servises;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Drive.v3.DriveService;

namespace AutoBackup.Http.GoogleDrive
{
    public class GoogleDriveHttpService : IGoogleDriveHttpService
    {
        string[] Scopes = { Scope.Drive };
        string ApplicationName = "Drive API .NET Quickstart";
        private readonly IFileService _fileService;
        public GoogleDriveHttpService(IFileService  fileService)
        {
            _fileService = fileService;
        }

        private string CreateFolder(string folderName)
        {
            var service = GetService();
           
            bool exists = Exists(folderName);
            if (exists)
                return $"Sorry but the file {folderName} already exists!";

            var file = new Google.Apis.Drive.v3.Data.File();
            file.Name = folderName;
            file.MimeType = "application/vnd.google-apps.folder";
            var request = service.Files.Create(file);
            request.Fields = "id";
            return request.Execute().Id;
        }

        private string CreateFolderParent(string folderName, string parent)
        {
            var service = GetService();
            var driveFolder = new Google.Apis.Drive.v3.Data.File();
            driveFolder.Name = folderName;
            driveFolder.MimeType = "application/vnd.google-apps.folder";
            driveFolder.Parents = new string[] { parent };
            var command = service.Files.Create(driveFolder);
            var file = command.Execute();
            return file.Id;
        }
        private bool Exists(string name)
        {

            var service = GetService();
            var listRequest = service.Files.List();
            listRequest.PageSize = 100;
            listRequest.Q = $"trashed = false and name contains '{name}' and 'root' in parents";
            listRequest.Fields = "files(name)";
            var files = listRequest.Execute().Files;

            foreach (var file in files)
            {
                if (name == file.Name)
                    return true;
            }
            return false;
        }


        private DriveService GetService()
        {

            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
            return service;
            //var tokenResponse = new TokenResponse
            //{
            //    AccessToken = "ya29.a0ARrdaM-aQzcVNETc0otNrkc5B8e6VPBpshfD3BzXk--LpBtDh0--oNpfnPcdRKwu8rMB1aYDr9_VzKsntGbKEOD24h6UN4Xtejb9LnfEEjxrFnS-6IUpp7WZoC1CgRp3iTeFzhUk80U5fGQkGJTernr90FoB",
            //    RefreshToken = "1//04zgxC-aTFVm_CgYIARAAGAQSNwF-L9Ir9-n6RopFf1a723nKm7YtbuI5QAeaSVeKRKlKOEAGPE9LpIhG1i3xj1XvIASlCSFkm7I",
            //};


            //var applicationName = "AutoBackup"; // Use the name of the project in Google Cloud

            //var username = "sobhan.plus.plus@gmail.com"; // Use your email

            //var apiCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            //{
            //    ClientSecrets = new ClientSecrets
            //    {
            //        ClientId = "786448905858-dmsfuqadacd6bha8qbtdfhv26ttgqih3.apps.googleusercontent.com",
            //        ClientSecret = "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"
            //    },
            //    Scopes = new[] { Scope.Drive },
            //    DataStore = new FileDataStore(applicationName)
            //});


            //var credential = new UserCredential(apiCodeFlow, username, tokenResponse);


            //var service = new DriveService(new BaseClientService.Initializer
            //{
            //    HttpClientInitializer = credential,
            //    ApplicationName = applicationName
            //});
            //return service;

        }

        private async Task<string> UploadFile(Stream file, string fileName, string fileMime, string folder, string fileDescription)
        {
            DriveService service = GetService();


            var driveFile = new Google.Apis.Drive.v3.Data.File();
            driveFile.Name = fileName;
            driveFile.Description = fileDescription;
            driveFile.MimeType = fileMime;
            driveFile.FolderColorRgb = "FFE600";
           driveFile.Parents = new string[] { folder };


            var request = service.Files.Create(driveFile, file, fileMime);
            request.Fields = "id";
            request.ProgressChanged += UploadProgessEvent;
        

            var response = await request.UploadAsync();
       
            if (response.Status != Google.Apis.Upload.UploadStatus.Completed)
                throw response.Exception;

            return request.ResponseBody.Id;
        }
        private void UploadProgessEvent(Google.Apis.Upload.IUploadProgress obj)
        {
         
         
            if (obj.Status == Google.Apis.Upload.UploadStatus.Uploading)
            {
                Console.WriteLine((obj.BytesSent * 100) / (5987426));
            }
        }
        private void DeleteFile(string fileId)
        {
            var service = GetService();
            var command = service.Files.Delete(fileId);
            var result = command.Execute();
        }

        IEnumerable<Google.Apis.Drive.v3.Data.File> GetFiles(string folder)
        {
            var service = GetService();

            var fileList = service.Files.List();
            fileList.Q = $"mimeType!='application/vnd.google-apps.folder' and '{folder}' in parents";
            fileList.Fields = "nextPageToken, files(id, name, size, mimeType)";

            var result = new List<Google.Apis.Drive.v3.Data.File>();
            string pageToken = null;
            do
            {
                fileList.PageToken = pageToken;
                var filesResult = fileList.Execute();
                var files = filesResult.Files;
                pageToken = filesResult.NextPageToken;
                result.AddRange(files);
            } while (pageToken != null);


            return result;
        }

        public void UploadDatabse(string fileName, string patch)
        {
            var isExit = Exists(fileName);
            if (!isExit) CreateFolder(fileName);
            var fileNameZip = _fileService.GetFileBaseNameUsingSplit(patch);
          
            var file = _fileService.GetFileStream(patch);
            UploadFile(file, fileNameZip, string.Empty, GetFolderIdByFolderName(fileName), "baakckcjdshjfs");

            //_fileService.DeleteFolder($"{Directory.GetCurrentDirectory()}\\Temp_{fileName}");
          
        }


        string GetFolderIdByFolderName(string folderName)
        {
            var service = GetService();
            FilesResource.ListRequest listRequest = service.Files.List();
            listRequest.PageSize = 10;
            listRequest.Q = "mimeType = 'application/vnd.google-apps.folder' and name = '"+folderName+"'";
            listRequest.Fields = "nextPageToken, files(id, name)";

            IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
            .Files;
            return files.First().Id;

       
        }
    }
}
