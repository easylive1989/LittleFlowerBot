using System;
using LittleFlowerBot.Models.GameExceptions;
using LittleFlowerBot.Models.Renderer;

namespace LittleFlowerBot.Models.Game.BoardGame
{
    public abstract class BoardGame<T> : Game where T : Enum
    {
        protected BoardGame(ITextRenderer textRenderer) : base(textRenderer)
        {
        }

        public override void Act(string userId, string cmd)
        {
            var player = new Player(userId);
            if (!GetBoard().IsTwoPlayers())
            {
                try
                {
                    GetBoard().Join(player);
                }
                catch (PlayerExistException)
                {
                    Render("你已經加入");
                }

                if (GetBoard().IsTwoPlayers())
                {
                    Render("遊戲開始");
                    
                    Render(GetBoard().GetBoardString());
                }
            }
            else
            {
                Move(cmd, player);
                
                Render(GetBoard().GetBoardString());

                if (GetBoard().IsGameOver())
                {
                    GameOver();
                }
            }
        }

        public virtual void GameOver()
        {
            Render("遊戲結束!");
        }

        protected virtual void Move(string cmd, Player player)
        {
            try
            {
                GetBoard().Move(player, cmd);
            }
            catch (NotYourTurnException)
            {
                Render("不是你的回合");
            }
            catch (CoordinateValidException)
            {
                Render("座標不合法");
            }
        }

        public override void StartGame()
        {
            Render("輸入++參加遊戲");
        }

        protected GameBoard<T> GetBoard()
        {
            return (GameBoard<T>)GameState;
        }
    }
}