using System;
using LittleFlowerBot.Models.BoardImage;
using LittleFlowerBot.Models.GameExceptions;

namespace LittleFlowerBot.Models.Game.BoardGame
{
    public abstract class BoardGame<T> : Game where T : Enum
    {
        protected abstract bool IsCmdValid(string cmd);

        public override void Act(string userId, string cmd)
        {
            var player = new Player(userId);
            if (cmd.Equals("++") && !GetBoard().IsPlayerFully())
            {
                try
                {
                    GetBoard().Join(player);
                }
                catch (PlayerExistException)
                {
                    Render("你已經加入");
                }

                if (GetBoard().IsPlayerFully())
                {
                    Render("遊戲開始");

                    RenderImage(BoardStateEncoder.EncodeToBytes(GetBoard()));
                }
            }
            else if(IsCmdValid(cmd) && GetBoard().IsPlayerFully())
            {
                Move(cmd, player);

                RenderImage(BoardStateEncoder.EncodeToBytes(GetBoard()));

                if (GetBoard().IsGameOver())
                {
                    GameOver();
                }
            }
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
            return (GameBoard<T>)GameBoard;
        }
    }
}