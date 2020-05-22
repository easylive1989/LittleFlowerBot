using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.Models.Game
{
    public abstract class Game
    {
        private readonly ITextRenderer _textRenderer;

        protected Game(ITextRenderer textRenderer)
        {
            _textRenderer = textRenderer;
        }

        public string SenderId { get; set; } = string.Empty;
        
        public IGameState GameState { get; set; }

        public abstract void StartGame();

        public abstract bool IsMatch(string cmd);

        public abstract void Act(string userId, string cmd);

        protected void Render(string text)
        {
            _textRenderer.Render(SenderId, text);
        }
    }
}