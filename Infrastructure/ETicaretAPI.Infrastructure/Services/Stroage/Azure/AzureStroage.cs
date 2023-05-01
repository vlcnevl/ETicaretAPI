using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ETicaretAPI.Application.Abstraction.Stroage.Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Stroage.Azure
{
    public class AzureStroage : IAzureStroage
    {
        readonly BlobServiceClient _blobServiceClient; // accounta bağlanır
        BlobContainerClient _blobContainerClient; // containera bağlanır

        public AzureStroage(IConfiguration configuration)
        {
            _blobServiceClient = new(configuration["Stroage:Azure"]);
        }


        public async Task DeleteAsync(string fileName, string containerName)
        {
           _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
           BlobClient blobClient =  _blobContainerClient.GetBlobClient(fileName);
           await blobClient.DeleteAsync();
        }

        public List<string> GetFiles(string containerName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Select(b=> b.Name).ToList();
        }

        public bool HasFile(string containerName, string fileName)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            return _blobContainerClient.GetBlobs().Any(b=> b.Name == fileName);
        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string containerName, IFormFileCollection files)
        {
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            await _blobContainerClient.CreateIfNotExistsAsync();
            await _blobContainerClient.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

            List<(string fileName, string pathOrContainerName)> datas = new();

            foreach (IFormFile file in files) 
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(file.Name);
               await blobClient.UploadAsync(file.OpenReadStream());
                datas.Add((file.Name,containerName)); 
            }
            return datas;
        }
    }
}
