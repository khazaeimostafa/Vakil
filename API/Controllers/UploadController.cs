using API.DTOs;
using Core.Interfaces;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
 using cc= Microsoft.AspNetCore.Hosting;

namespace API.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileStorageRepository fileStorageRepository;
        private readonly cc.IWebHostEnvironment env;
        public UploadController(cc.IWebHostEnvironment env, IFileStorageRepository fileStorageRepository)
        {
            this.fileStorageRepository = fileStorageRepository;
            this.env = env;
        }

        [HttpPost("Create")]
        public async Task<ActionResult> uploadImage(IFormFile model)
        {
            if (model  != null)
            {
                string routeForDB =
                    await fileStorageRepository
                        .SaveFile("Carousel", model         );

                return Ok(routeForDB);
            }
            return BadRequest("Ohh Shit");
        }
    }
}
