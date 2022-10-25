using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IFileStorageRepository
    {
        Task DeleteFile(string fileRoute, string containerName);

        Task<string> SaveFile(string containerName, IFormFile file);

        Task<string>
        EditFile(
            string containerName, IFormFile file, string fileRoute
        );
    }
}
