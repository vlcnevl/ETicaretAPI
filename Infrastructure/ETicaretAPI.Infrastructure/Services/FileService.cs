using ETicaretAPI.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class FileService : IFileService
    {
        readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                //Ifiledisposabledan türediği için using.İşi bitince dispose edecek 
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);

                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch (Exception ex) 
            {
                throw ex;
            }
        }

        public async Task<string> FileRenameAsync(string filePath, string fileName)
        {
            string newFileName = Regex.Replace(fileName, @"[\s&()]+", "-"); // Boşluk, ., &, ( ve ) gibi karakterleri '-' işaretine dönüştürür.

            string fullPath = Path.Combine(filePath, newFileName);

            if (!File.Exists(fullPath))
            {
                return newFileName;
            }

            int fileCount = 1;
            string extension = Path.GetExtension(newFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);

            while (File.Exists(fullPath))
            {
                newFileName = fileNameWithoutExtension + "-" + fileCount.ToString() + extension;

                fullPath = Path.Combine(filePath, newFileName);
                fileCount++;
            }

            return newFileName;

        }

        public async Task<List<(string fileName, string path)>> UploadAsync(string path, IFormFileCollection files)
        {
            // weebrootpath wwwroot u getirir direkt
            string uploadPath = Path.Combine(_environment.WebRootPath, path);
            if (!Directory.Exists(uploadPath)) 
                Directory.CreateDirectory(uploadPath);

            List<bool> results = new();
            List<(string fileName,string path)> datas = new();

            foreach (IFormFile file in files) {
              string fileName = await FileRenameAsync(uploadPath,file.FileName);
               bool result =  await CopyFileAsync($"{uploadPath}\\{fileName}",file);
                datas.Add((fileName, $"{uploadPath}\\{fileName}")); 
            }


            if (results.TrueForAll(result => result.Equals(true)))
                return datas;

            return null;
            //todo hatalara  bakılacak     
        


            //basit dosya yükleme
            // //wwwroot/resource/productImages
            // string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath,"resource/productImages");


            // if(!Directory.Exists(uploadPath))
            //     Directory.CreateDirectory(uploadPath);

            // Random rand = new();
            // foreach (IFormFile file in Request.Form.Files)
            // {
            //     string fullPath = Path.Combine(uploadPath,$"{rand.Next()}{Path.GetExtension(file.FileName)}");

            //     using FileStream fileStream = new(fullPath, FileMode.Create, FileAccess.Write, FileShare.None,1024*1024,useAsync:false);

            //     await file.CopyToAsync(fileStream);
            //     await fileStream.FlushAsync();

        }

    }
}
