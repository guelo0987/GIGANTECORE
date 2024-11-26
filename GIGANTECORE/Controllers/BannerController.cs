using Microsoft.AspNetCore.Mvc;
using GIGANTECORE.Context;
using GIGANTECORE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PassHash;

namespace GIGANTECORE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BannerController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly IWebHostEnvironment _environment;

        public BannerController(MyDbContext db, IWebHostEnvironment environment)
        {
            _db = db;
            _environment = environment;
            

            
        }

        [HttpGet]
        public IActionResult GetBanner()
        {
            retu
        }



        [HttpPost]
        public IActionResult UploadBanner(IFormFile file)
        {
            return Ok(new AdminMultiMedia(_db).Upload(file));

        }

    }
}