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

        public ITextRenderer Get(string? replyToken)
        {
            if (_environment.IsDevelopment())
            {
                return GetRenderer<ConsoleRenderer>();
            }

            var renderer = GetRenderer<BufferedReplyRenderer>();
            renderer.ReplyToken = replyToken;
            return renderer;
        }

        private T GetRenderer<T>() where T : ITextRenderer
        {
            return (T)(_serviceProvider.GetService(typeof(T)) ?? throw new InvalidOperationException($"Service {typeof(T)} is not registered."));
        }
    }
}
