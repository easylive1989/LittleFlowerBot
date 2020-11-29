using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace LittleFlowerBot.Models.Renderer
{
    public class RendererFactory : IRendererFactory
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IServiceProvider _serviceProvider;

        public RendererFactory(IWebHostEnvironment environment, IServiceProvider serviceProvider)
        {
            _environment = environment;
            _serviceProvider = serviceProvider;
        }
        
        public ITextRenderer Get(string senderId)
        {
            if (_environment.IsDevelopment())
            {
                return GetRenderer<ConsoleRenderer>();
            }

            var lineNotifySender = GetRenderer<LineNotifySender>();
            lineNotifySender.SenderId = senderId;
            return lineNotifySender;
        }

        private T GetRenderer<T>() where T : ITextRenderer
        {
            return (T)_serviceProvider.GetService(typeof(T));
        }
    }
}