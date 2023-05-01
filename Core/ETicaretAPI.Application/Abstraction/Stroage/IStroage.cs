using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstraction.Stroage
{
    public interface IStroage
    {
        Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files);
        Task DeleteAsync(string fileName,string pathOrContainerName);
        List<string> GetFiles(string pathOrContainerName);
        bool HasFile(string pathOrContainerName,string fileName);

    }
}
