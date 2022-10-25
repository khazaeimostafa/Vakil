using Core.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class AppStorageRepository : IFileStorageRepository
    {
        private readonly IHostingEnvironment  env;
        private readonly IHttpContextAccessor httpContextAccessor;


         

        public AppStorageRepository(IHostingEnvironment  env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteFile(string fileRoute, string containerName)
        {
            throw new NotImplementedException();
        }

        public Task<string>
        EditFile(string containerName, IFormFile file, string fileRoute)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveFile(string containerName, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            var folder =
                Path.Combine(env.ContentRootPath, containerName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string route = Path.Combine(folder, fileName);

            using (MemoryStream? ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                byte[]? content = ms.ToArray();

                await File.WriteAllBytesAsync(route, content);

            }


            var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var routeForDB = Path.Combine(url, containerName, fileName).Replace("\\", "/");
            return routeForDB;


        }

    }
}
