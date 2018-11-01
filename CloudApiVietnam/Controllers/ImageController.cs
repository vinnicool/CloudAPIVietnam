using CloudApiVietnam.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using AllowAnonymousAttribute = System.Web.Http.AllowAnonymousAttribute;

namespace CloudApiVietnam.Controllers
{
    [System.Web.Http.Authorize]
    public class ImagesController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // To be set AzureStorage if not, sql database will be used to store images (less than 32gb)
        private string ImageStoragetype = "AzureStorage"; 
        // GET specifieke Image
        [AllowAnonymous]
        public async Task<Stream> GetAsync(string reference)
        {
            Stream imageStream = null;
            if (ImageStoragetype == "AzureStorage")
            {
                
                var container = getStorageAccount();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(reference);
                await blockBlob.DownloadToStreamAsync(imageStream);

            }
            else
            {
                Image image = db.Image.Where(f => f.name == reference).FirstOrDefault();
                imageStream = image.image;
            }

            return imageStream;
        }

        // GET alle images Image
        public Image Get(int id)
        {
            return null;
        }

        // POST een Image
        [AllowAnonymous]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                Stream imageStream = await file.ReadAsStreamAsync();
                var blobNameReference = $"{Guid.NewGuid().ToString()}";
                if (ImageStoragetype == "AzureStorage")
                {   
                    await AzureStorageAsync(imageStream, blobNameReference);
                    return Content(HttpStatusCode.Created, blobNameReference);
                }
                else
                {
                    SqlStorage(imageStream,blobNameReference);
                    return Content(HttpStatusCode.Created, blobNameReference);
                }
            }

            return Ok();
        }

        private async Task AzureStorageAsync(Stream image, string reference) {

            var container = getStorageAccount();
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            // Get Blob
            var blob = container.GetBlockBlobReference(reference);

            // Upload image
            await blob.UploadFromStreamAsync(image);
        }

        private CloudBlobContainer getStorageAccount() {
            // Get storage account
            CloudStorageAccount storageAccount = new CloudStorageAccount(
            new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("vietnamcloud", "3yD7TzCjwYab4BKJvxSjX5EVvihI3gz1FGvRehtJUch3JTqWku1TLv8nRrASdXCxKrI/7GzbiHiZOtd8QuEJ5g=="), true);

            // Create Blob reference
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("patientimages");
            return container;
        }

        // DELETE api/values/5
        public async Task DeleteAsync(string reference)
        {
            if (ImageStoragetype == "AzureStorage")
            {
                var container = getStorageAccount();
                // Get a reference to a blob named "myblob.txt".
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(reference);
                // Delete the blob.
                await blockBlob.DeleteAsync();
            }
            else
            {
                var image = db.Image.Where(f => f.name == reference).FirstOrDefault();
                db.Image.Remove(image);
                db.SaveChanges();
            }
        }
        
        private void SqlStorage(Stream imageStream, string blobNameReference)
        {
            Image image = new Image();
            image.name = blobNameReference;
            image.image = imageStream;

            db.Image.Add(image);
            db.SaveChanges();
        }
    }
}
