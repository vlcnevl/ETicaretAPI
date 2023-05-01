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
    public class FileService 
    {
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

    }
}
