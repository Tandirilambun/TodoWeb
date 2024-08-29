using Firebase.Storage;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TodoWeb.Services
{
    public class FirebaseStorageService
    {
        private readonly FirebaseConfig _firebaseConfig;

        public FirebaseStorageService(IOptions<FirebaseConfig> firebaseConfig)
        {
            _firebaseConfig = firebaseConfig.Value;
        }

        public async Task<string> UploadFileAsync(IFormFile file, string userId, string fileName, string contentType)
        {
            var storage = StorageClient.Create();

            using(var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadObject = await storage.UploadObjectAsync(_firebaseConfig.StorageBucket, fileName, contentType, memoryStream);
                //return uploadObject.MediaLink;

                var firebaseStorage = new FirebaseStorage(_firebaseConfig.StorageBucket);

                var downloadUrl = await firebaseStorage
                    .Child("profile-pictures")
                    .Child(userId)
                    .GetDownloadUrlAsync();
                return downloadUrl;
            }
        }

        //public async Task<string> GetDownloadUrlAsync(string fileName)
        //{
        //    var storage = new FirebaseStorage(_firebaseConfig.StorageBucket);

        //    var downloadUrl = await storage
        //        .Child("profile-pictures")
        //        .Child(fileName)
        //        .GetDownloadUrlAsync();

        //    return downloadUrl;
        //}
    }
}
