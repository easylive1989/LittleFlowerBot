using System;
using LittleFlowerBot.Models.BoardImage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LittleFlowerBot.Controllers
{
    [ApiController]
    [Route("api/board-images")]
    public class BoardImageController : ControllerBase
    {
        private readonly ILogger<BoardImageController> _logger;

        public BoardImageController(ILogger<BoardImageController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{encodedState}")]
        public IActionResult GetImage(string encodedState)
        {
            try
            {
                var imageData = BoardStateEncoder.DecodeAndRender(encodedState);
                _logger.LogInformation("Board image rendered successfully, size: {Size} bytes, encodedState: {EncodedState}", imageData.Length, encodedState);
                return File(imageData, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to render board image, encodedState: {EncodedState}", encodedState);
                return NotFound();
            }
        }
    }
}
