using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Stroage
{
    public class Stroage 
    {
        protected delegate bool HasFile(string pathOrContainerName,string fileName);
        protected async Task<string> FileRenameAsync(string pathOrContainerName, string fileName, HasFile hasFileMethod) //kalıtımsal olarak erişilsin diye 
        {
            string newFileName = Regex.Replace(fileName, @"[\s&()]+", "-"); // Boşluk, ., &, ( ve ) gibi karakterleri '-' işaretine dönüştürür.

            string fullPath = Path.Combine(pathOrContainerName, newFileName);

            if (!hasFileMethod(pathOrContainerName,newFileName))
            {
                return newFileName;
            }

            int fileCount = 1;
            string extension = Path.GetExtension(newFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(newFileName);

            while (hasFileMethod(pathOrContainerName, newFileName))
            {
                newFileName = fileNameWithoutExtension + "-" + fileCount.ToString() + extension;

                fullPath = Path.Combine(pathOrContainerName, newFileName);
                fileCount++;
            }

            return newFileName;

        }

    }
}
