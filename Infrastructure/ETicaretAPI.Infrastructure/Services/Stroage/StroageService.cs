using ETicaretAPI.Application.Abstraction.Stroage;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Stroage
{
    public class StroageService : IStroageService
    {
        readonly IStroage _stroage;

        public StroageService(IStroage stroage)
        {
            _stroage = stroage;
        }

        public string StroageName { get => _stroage.GetType().Name; }

        public async Task DeleteAsync(string fileName, string pathOrContainerName) => await _stroage.DeleteAsync(fileName, pathOrContainerName);

        public List<string> GetFiles(string pathOrContainerName) => _stroage.GetFiles(pathOrContainerName);

        public bool HasFile(string pathOrContainerName, string fileName) => _stroage.HasFile(pathOrContainerName, fileName);

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files)
        => await _stroage.UploadAsync(pathOrContainerName, files);    
    }
}
