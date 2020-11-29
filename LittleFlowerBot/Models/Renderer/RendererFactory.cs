using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LittleFlowerBot.Models.Renderer
{
    public class RendererFactory : IRendererFactory
    {
        private readonly IWebHostEnvironment _environment;
        private readonly LineNotifySender _lineNotifySender;
        private readonly ConsoleRenderer _consoleRenderer;

        public RendererFactory(IWebHostEnvironment environment, LineNotifySender lineNotifySender, ConsoleRenderer consoleRenderer)
        {
            _environment = environment;
            _lineNotifySender = lineNotifySender;
            _consoleRenderer = consoleRenderer;
        }
        
        public ITextRenderer Get(string senderId)
        {
            if (_environment.IsDevelopment())
            {
                return _consoleRenderer;
            }
           
            _lineNotifySender.SenderId = senderId;
            return _lineNotifySender;
        }
    }
}