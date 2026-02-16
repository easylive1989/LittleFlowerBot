using System;
using LittleFlowerBot.Models.BoardImage;
using Microsoft.AspNetCore.Mvc;

namespace LittleFlowerBot.Controllers
{
    [ApiController]
    [Route("api/board-images")]
    public class BoardImageController : ControllerBase
    {
        [HttpGet("{encodedState}")]
        public IActionResult GetImage(string encodedState)
        {
            try
            {
                var imageData = BoardStateEncoder.DecodeAndRender(encodedState);
                return File(imageData, "image/png");
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}
