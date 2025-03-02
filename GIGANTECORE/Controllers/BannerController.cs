using Microsoft.AspNetCore.Mvc;
using GIGANTECORE.Context;
using GIGANTECORE.Models;
using GIGANTECORE.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> UploadBanner(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest(new { success = false, message = "No se ha proporcionado ningún archivo" });
            }

            var result = await new AdminMultiMedia(_db).Upload(file);
            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            var result = await new AdminMultiMedia(_db).Delete(id);
            
            if (!result)
            {
                return NotFound(new { success = false, message = "Banner no encontrado o no se pudo eliminar" });
            }

            return Ok(new { success = true, message = "Banner eliminado exitosamente" });
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