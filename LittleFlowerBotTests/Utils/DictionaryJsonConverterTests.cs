using System.Text.Json;
using ExpectedObjects;
using LittleFlowerBot.Models.Game.BoardGame;
using LittleFlowerBot.Models.Game.BoardGame.KiGames;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Utils;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace LittleFlowerBotTests.Utils
{
    [TestFixture]
    public class DictionaryJsonConverterTests
    {

        [Test]
        public void Convert_TicTacToeBoard_To_Json()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new DictionaryJsonConverter<Ki, Player>());

            ClassicAssert.AreEqual("{\"CurMoveX\":0,\"CurMoveY\":0,\"CurPlayer\":null,\"Row\":3,\"Column\":3,\"GameBoardArray\":[[0,0,0],[0,0,0],[0,0,0]],\"PlayerMap\":{\"1\":null,\"2\":null},\"PlayerMoveOrder\":[]}", JsonSerializer.Serialize(new TicTacToeBoard(), jsonSerializerOptions));
        }

        [Test]
        public void Convert_Json_To_TicTacToeBoard()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new DictionaryJsonConverter<Ki, Player>());
 
            new TicTacToeBoard().ToExpectedObject().ShouldEqual(JsonSerializer.Deserialize<TicTacToeBoard>("{\"CurMoveX\":0,\"CurMoveY\":0,\"CurPlayer\":null,\"Row\":3,\"Column\":3,\"GameBoardArray\":[[0,0,0],[0,0,0],[0,0,0]],\"PlayerMap\":{\"1\":null,\"2\":null},\"PlayerMoveOrder\":[]}", jsonSerializerOptions));
        }
    }
}