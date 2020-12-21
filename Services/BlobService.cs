using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BlobStorageDemo.Models;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlobInfo = BlobStorageDemo.Models.BlobInfo;

namespace BlobStorageDemo.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task<BlobInfo> GetBlobAsync(string name)
        {
            var client = _blobServiceClient.GetBlobContainerClient("blobupload");
            var blobClient = client.GetBlobClient(name);
            var blobDownload = await blobClient.DownloadAsync();

            return new BlobInfo(blobDownload.Value.Content, blobDownload.Value.ContentType);
        }

        public async Task<IEnumerable<string>> ListBlobsAsync()
        {
            var client = _blobServiceClient.GetBlobContainerClient("blobupload");
            var itemList = new List<string>();

            await foreach(var item in client.GetBlobsAsync())
            {
                itemList.Add(item.Name);
            }

            return itemList;
        }

        public async Task UploadContentBlobAsync(string content, string fileName)
        {
            var client = _blobServiceClient.GetBlobContainerClient("blobupload");
            var blobClient = client.GetBlobClient(fileName);
            var bytes = Encoding.UTF8.GetBytes(content);
            using var memoryStream = new MemoryStream(bytes);
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders { ContentType = fileName.GetContentType() });
        }


        // Tried to add the extenstion directly here, but since in .net core it's not alloawed to create the extension method and must be defined in a non generic class.

        // Hence Util helper class for proper abstraction purposes.

        /*
        private static readonly FileExtensionContentTypeProvider Provider = new FileExtensionContentTypeProvider();

        public static string GetContentType(this string fileName)
        {
            if (!Provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        */
        public async Task UploadFileBlobAsync(string filePath, string fileName)
        {
            var client = _blobServiceClient.GetBlobContainerClient("blobupload");
            var blobClient = client.GetBlobClient(fileName);
            await blobClient.UploadAsync(filePath, new BlobHttpHeaders { ContentType = filePath.GetContentType()});

        }

        public async Task DeleteBlobAsync(string blobName)
        {
            var client = _blobServiceClient.GetBlobContainerClient("blobupload");
            var blobClient = client.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }

    }
}
