using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using isRock.LineBot;
using LittleFlowerBot.Extensions;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.GameResult;
using LittleFlowerBot.Models.Renderer;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Services.EventHandler
{
    public class RecordHandler : ILineEventHandler
    {
        private readonly ITextRenderer _textRenderer;
        private readonly IBoardGameResultsRepository _boardGameResultsRepository;
        
        private readonly Dictionary<GameType, string> _gameNameDisplayDict = new Dictionary<GameType, string>()
        {
            {GameType.Gomoku, "五子棋"},
            {GameType.TicTacToe, "井字遊戲"}
        };

        public RecordHandler(ITextRenderer textRenderer, IBoardGameResultsRepository boardGameResultsRepository)
        {
            _textRenderer = textRenderer;
            _boardGameResultsRepository = boardGameResultsRepository;
        }

        public async Task Act(Event @event)
        {
            if (!@event.Text().Equals("我的戰績"))
            {
                return;
            }
            
            var dualPlayerGameResults = await _boardGameResultsRepository.GetResult(@event.UserId());
            var gameResults = dualPlayerGameResults.GroupBy(result => result.GameType).Select(results => new
            {
                GameName = _gameNameDisplayDict[results.Key],
                WinCount = results.Count(result => result.Result == GameResult.Win),
                LostCount = results.Count(result => result.Result == GameResult.Lose),
                DrawCount = results.Count(result => result.Result == GameResult.Draw),
            });

            if (!gameResults.Any())
            {
                _textRenderer.Render("你沒有任何戰績");
                return;
            }
            
            var stringBuilder = new StringBuilder();
            foreach (var gameResult in gameResults)
            {
                stringBuilder.Append($"你在{gameResult.GameName}贏了{gameResult.WinCount}次，輸了{gameResult.LostCount}次\n");
            }
            _textRenderer.Render(stringBuilder.ToString());
        }
    }
}