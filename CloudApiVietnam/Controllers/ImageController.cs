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
using AuthorizeAttribute = System.Web.Http.AuthorizeAttribute;
using System.Web.Management;
using Newtonsoft.Json;
using RouteAttribute = System.Web.Http.RouteAttribute;
using RoutePrefixAttribute = System.Web.Http.RoutePrefixAttribute;
using System.Net.Http.Headers;

namespace CloudApiVietnam.Controllers
{
    [Authorize]
    public class ImagesController : ApiController
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // To be set AzureStorage if not, sql database will be used to store images (less than 32gb)
        private string ImageStoragetype = "AzureStorage";
        // GET specifieke Image
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Get(string id)
        {
            //string reference = "55c6df2c-d258-aa05-b9f96bb11de2.jpeg";
            MemoryStream imageStream = new MemoryStream();
            HttpResponseMessage result;
            if (ImageStoragetype == "AzureStorage")
            {

                CloudBlobContainer container = getStorageAccount();
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);
                try
                {
                    await blockBlob.DownloadToStreamAsync(imageStream);
                    imageStream.Position = 0;
                    if (imageStream == null) {
                        result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        result.Content = new StringContent("Requested image could not be downloaded..");
                        return result;
                    }
                }
                catch(Exception e)
                {
                    if (e.Message.Contains("(404)")) {
                        result = new HttpResponseMessage(HttpStatusCode.NotFound);
                        result.Content = new StringContent("Image not found");
                        return result;
                    } else {
                        result = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        result.Content = new StringContent(e.Message);
                        return result;
                    }
                    
                }
            }
            else
            {
                Image image = db.Image.Where(f => f.name == id).FirstOrDefault();
                //imageStream. = image.image;
            }

            result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new StreamContent(imageStream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return result;
        }
        // POST een Image
        [AllowAnonymous]
        public async Task<IHttpActionResult> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            var provider = new MultipartMemoryStreamProvider();

            try
            {
                await Request.Content.ReadAsMultipartAsync(provider);
            }
            catch (Exception e)
            {
                return CheckIfFileIsToBig(e);

            }
            List<Image> imageList = new List<Image>();
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                // Checking for file type and size
                string imageType = filename.Split('.')[1];
                var types = new List<string> { "jpeg", "jpg", "png" };
                bool contains = types.Contains(imageType, StringComparer.OrdinalIgnoreCase);
                if (!contains)
                {
                    return Content(HttpStatusCode.BadRequest, "Types of Digital Image should be: jpeg, jpg or png");
                }
                Stream imageStream = await file.ReadAsStreamAsync();
                // Creates a unique value
                var blobNameReference = $"{Guid.NewGuid().ToString()}";
                if (ImageStoragetype == "AzureStorage")
                {
                    try
                    {
                        await AzureStorageAsync(imageStream, blobNameReference);
                        Image image = new Image();
                        image.name = blobNameReference;
                        imageList.Add(image);
                    }
                    catch (Exception e)
                    {
                        return Content(HttpStatusCode.InternalServerError, e.Message);
                    }

                }
                else
                {
                    try
                    {
                        //SqlStorage(imageStream, blobNameReference);
                        //imageList.Add(blobNameReference);

                    }
                    catch (Exception e)
                    {
                        return Content(HttpStatusCode.InternalServerError, e.Message);
                    }
                }
            }
            return Content(HttpStatusCode.Created, imageList);

        }

        private IHttpActionResult CheckIfFileIsToBig(Exception e)
        {
            List<string> errors = new List<string> { "De maximale aanvraaglengte is overschreden.", "Maximum request length exceeded.", "Độ dài yêu cầu tối đa đã bị vượt quá" };
            bool contains = errors.Contains(e.InnerException.Message, StringComparer.OrdinalIgnoreCase);
            if (contains)
            {
                return Content(HttpStatusCode.RequestEntityTooLarge, "Image size should be smaller than 5,0 MBytes");
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }
        }

        private async Task AzureStorageAsync(Stream image, string reference)
        {

            var container = getStorageAccount();
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            // Get Blob
            var blob = container.GetBlockBlobReference(reference);

            // Upload image
            await blob.UploadFromStreamAsync(image);
        }

        private CloudBlobContainer getStorageAccount()
        {
            // Get storage account
            CloudStorageAccount storageAccount = new CloudStorageAccount(
            new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("vietnamcloud", "3yD7TzCjwYab4BKJvxSjX5EVvihI3gz1FGvRehtJUch3JTqWku1TLv8nRrASdXCxKrI/7GzbiHiZOtd8QuEJ5g=="), true);

            // Create Blob reference
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("patientimages");
            return container;
        }

        // DELETE api/values/5
        [AllowAnonymous]
        public async Task Delete(string id)
        {//string reference
            //string reference = "1c836831-4d83-4bcd-99cc-29907e12942a.jpeg";
            if (ImageStoragetype == "AzureStorage")
            {
                var container = getStorageAccount();
                // Get a reference to a blob named "myblob.txt".
                
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);
                // Delete the blob.
                await blockBlob.DeleteAsync();
            }
            else
            {
                var image = db.Image.Where(f => f.name == id).FirstOrDefault();
                db.Image.Remove(image);
                db.SaveChanges();
            }
        }

        private void SqlStorage(MemoryStream imageStream, string blobNameReference)
        {
            Image image = new Image();
            image.name = blobNameReference;
            image.image = imageStream;

            db.Image.Add(image);
            db.SaveChanges();
        }
    }
}
