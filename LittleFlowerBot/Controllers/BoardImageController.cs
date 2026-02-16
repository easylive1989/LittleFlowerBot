using LittleFlowerBot.Models.BoardImage;
using Microsoft.AspNetCore.Mvc;

namespace LittleFlowerBot.Controllers
{
    [ApiController]
    [Route("api/board-images")]
    public class BoardImageController : ControllerBase
    {
        private readonly IBoardImageStore _imageStore;

        public BoardImageController(IBoardImageStore imageStore)
        {
            _imageStore = imageStore;
        }

        [HttpGet("{imageId}")]
        public IActionResult GetImage(string imageId)
        {
            var imageData = _imageStore.Get(imageId);
            if (imageData == null)
            {
                return NotFound();
            }

            return File(imageData, "image/png");
        }
    }
}
