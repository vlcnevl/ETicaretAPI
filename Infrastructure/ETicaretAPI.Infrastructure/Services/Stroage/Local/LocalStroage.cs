using ETicaretAPI.Application.Abstraction.Stroage.LocalStroage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Stroage.Local
{
    public class LocalStroage : ILocalStroage
    {
        readonly IWebHostEnvironment _environment;

        public LocalStroage(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task DeleteAsync(string fileName, string path) => File.Delete($"{path}\\{fileName}");
        

        public List<string> GetFiles(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
           return directory.GetFiles().Select(f=> f.Name).ToList();
        }

        public bool HasFile(string path, string fileName) => File.Exists($"{path}\\{fileName}");


        async Task<bool> CopyFileAsync(string path, IFormFile file)
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


        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files)
        {
            // weebrootpath wwwroot u getirir direkt
            string uploadPath = Path.Combine(_environment.WebRootPath, path);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            
            List<(string fileName, string path)> datas = new();

            foreach (IFormFile file in files)
            {
                await CopyFileAsync($"{uploadPath}\\{file.Name}", file);
                datas.Add((file.Name, $"{path}\\{file.Name}"));
            }


             return datas;
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
