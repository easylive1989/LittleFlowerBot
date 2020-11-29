using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LittleFlowerBot.Models.Renderer
{
    public class RendererFactory : IRendererFactory
    {
        private readonly IWebHostEnvironment _environment;
        private readonly LineNotify _lineNotify;
        private readonly ConsoleRenderer _consoleRenderer;

        public RendererFactory(IWebHostEnvironment environment, LineNotify lineNotify, ConsoleRenderer consoleRenderer)
        {
            _environment = environment;
            _lineNotify = lineNotify;
            _consoleRenderer = consoleRenderer;
        }
        
        public ITextRenderer Get(string senderId)
        {
            if (_environment.IsDevelopment())
            {
                return _consoleRenderer;
            }
           
            _lineNotify.SenderId = senderId;
            return _lineNotify;
        }
    }
}