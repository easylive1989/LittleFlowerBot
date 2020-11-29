using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.Models.Game
{
    public abstract class Game
    {
        public ITextRenderer TextRenderer { get; set; }

        public IGameBoard GameBoard { get; set; }

        public abstract void Act(string userId, string cmd);

        public abstract bool IsMatch(string cmd);

        public abstract void StartGame();

        protected void Render(string text)
        {
            TextRenderer.Render(text);
        }

        public virtual void GameOver()
        {
            Render("遊戲結束!");
        }
    }
}