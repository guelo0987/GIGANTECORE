using Microsoft.AspNetCore.Mvc;
using GIGANTECORE.Context;
using GIGANTECORE.Models;
using GIGANTECORE.Services;
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
            return Ok(new AdminMultiMedia(_db).GetImages());
        }



        [HttpPost]
        public IActionResult UploadBanner(IFormFile file)
        {
            return Ok(new AdminMultiMedia(_db).Upload(file));

        }
        
        [HttpDelete("{id}")]
        public IActionResult DeleteBanner(int id)
        {
            return Ok(new AdminMultiMedia(_db).Delete(id));

        }
        
        [HttpPut]
        public IActionResult ReorderBanners([FromBody] List<BannerOrderDto> newOrders)
        {
            var ordersList = newOrders.Select(x => (x.Id, x.NewOrder)).ToList();
            var result = new AdminMultiMedia(_db).ReorderImages(ordersList);
        
            if (!result)
            {
                return BadRequest("No se pudieron reordenar las imágenes");
            }
        
            return Ok("Imágenes reordenadas exitosamente");
        }

        [HttpPut("{id}")]
        public IActionResult ToggleActiveBanner(int id)
        {
            var result = new AdminMultiMedia(_db).ToggleActive(id);
        
            if (!result)
            {
                return NotFound("Banner no encontrado");
            }
        
            return Ok("Estado del banner actualizado exitosamente");
        }

    }
}