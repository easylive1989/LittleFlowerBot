using System;
using LittleFlowerBot.Models.Game;
using LittleFlowerBot.Models.Game.BoardGame.KiGames;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.TicTacToe;
using LittleFlowerBot.Models.Game.BoardGame.KiGames.Gomoku;
using LittleFlowerBot.Models.Game.BoardGame.ChessGames.ChineseChess;

namespace LittleFlowerBot.Models.BoardImage
{
    public static class BoardStateEncoder
    {
        private const byte TypeTicTacToe = 0;
        private const byte TypeGomoku = 1;
        private const byte TypeChineseChess = 2;

        public static byte[] EncodeToBytes(IGameBoard board)
        {
            return board switch
            {
                TicTacToeBoard ttt => EncodeKiBoard(TypeTicTacToe, ttt),
                GomokuBoard gomoku => EncodeGomokuBoard(gomoku),
                ChineseChessBoard chess => EncodeChineseChessBoard(chess),
                _ => throw new ArgumentException($"Unknown board type: {board.GetType()}")
            };
        }

        public static byte[] DecodeAndRender(string encoded)
        {
            var bytes = Base64UrlDecode(encoded);
            if (bytes.Length == 0)
                throw new ArgumentException("Empty encoded state");

            return bytes[0] switch
            {
                TypeTicTacToe => DecodeAndRenderKiBoard<TicTacToeBoard>(bytes),
                TypeGomoku => DecodeAndRenderGomokuBoard(bytes),
                TypeChineseChess => DecodeAndRenderChineseChessBoard(bytes),
                _ => throw new ArgumentException($"Unknown board type: {bytes[0]}")
            };
        }

        public static string Base64UrlEncode(byte[] bytes)
        {
            return Convert.ToBase64String(bytes)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string encoded)
        {
            var padded = encoded.Replace('-', '+').Replace('_', '/');
            switch (padded.Length % 4)
            {
                case 2: padded += "=="; break;
                case 3: padded += "="; break;
            }
            return Convert.FromBase64String(padded);
        }

        private static byte[] EncodeKiBoard(byte type, KiBoard board)
        {
            var cellCount = board.Row * board.Column;
            var packedBytes = (cellCount + 3) / 4;
            var bytes = new byte[1 + packedBytes];
            bytes[0] = type;

            var idx = 0;
            for (var i = 0; i < board.Row; i++)
            {
                for (var j = 0; j < board.Column; j++)
                {
                    var val = (int)board.GameBoardArray[i][j];
                    var byteIdx = 1 + idx / 4;
                    var shift = (idx % 4) * 2;
                    bytes[byteIdx] |= (byte)(val << shift);
                    idx++;
                }
            }
            return bytes;
        }

        private static byte[] EncodeGomokuBoard(GomokuBoard board)
        {
            var cellCount = board.Row * board.Column;
            var packedBytes = (cellCount + 3) / 4;
            var bytes = new byte[1 + 2 + packedBytes];
            bytes[0] = TypeGomoku;
            bytes[1] = (byte)board.CurMoveX;
            bytes[2] = (byte)board.CurMoveY;

            var idx = 0;
            for (var i = 0; i < board.Row; i++)
            {
                for (var j = 0; j < board.Column; j++)
                {
                    var val = (int)board.GameBoardArray[i][j];
                    var byteIdx = 3 + idx / 4;
                    var shift = (idx % 4) * 2;
                    bytes[byteIdx] |= (byte)(val << shift);
                    idx++;
                }
            }
            return bytes;
        }

        private static byte[] EncodeChineseChessBoard(ChineseChessBoard board)
        {
            var cellCount = board.Row * board.Column;
            var packedBytes = (cellCount + 1) / 2;
            var bytes = new byte[1 + packedBytes];
            bytes[0] = TypeChineseChess;

            var idx = 0;
            for (var i = 0; i < board.Row; i++)
            {
                for (var j = 0; j < board.Column; j++)
                {
                    var val = (int)board.GameBoardArray[i][j];
                    var byteIdx = 1 + idx / 2;
                    var shift = (idx % 2) * 4;
                    bytes[byteIdx] |= (byte)(val << shift);
                    idx++;
                }
            }
            return bytes;
        }

        private static byte[] DecodeAndRenderKiBoard<TBoard>(byte[] bytes) where TBoard : KiBoard, new()
        {
            var board = new TBoard();
            var idx = 0;
            for (var i = 0; i < board.Row; i++)
            {
                for (var j = 0; j < board.Column; j++)
                {
                    var byteIdx = 1 + idx / 4;
                    var shift = (idx % 4) * 2;
                    var val = (bytes[byteIdx] >> shift) & 0x03;
                    board.GameBoardArray[i][j] = (Ki)val;
                    idx++;
                }
            }
            return board.GetBoardImage();
        }

        private static byte[] DecodeAndRenderGomokuBoard(byte[] bytes)
        {
            var board = new GomokuBoard();
            board.CurMoveX = bytes[1];
            board.CurMoveY = bytes[2];

            var idx = 0;
            for (var i = 0; i < board.Row; i++)
            {
                for (var j = 0; j < board.Column; j++)
                {
                    var byteIdx = 3 + idx / 4;
                    var shift = (idx % 4) * 2;
                    var val = (bytes[byteIdx] >> shift) & 0x03;
                    board.GameBoardArray[i][j] = (Ki)val;
                    idx++;
                }
            }
            return board.GetBoardImage();
        }

        private static byte[] DecodeAndRenderChineseChessBoard(byte[] bytes)
        {
            var board = new ChineseChessBoard();
            var idx = 0;
            for (var i = 0; i < board.Row; i++)
            {
                for (var j = 0; j < board.Column; j++)
                {
                    var byteIdx = 1 + idx / 2;
                    var shift = (idx % 2) * 4;
                    var val = (bytes[byteIdx] >> shift) & 0x0F;
                    board.GameBoardArray[i][j] = (ChineseChess)val;
                    idx++;
                }
            }
            return board.GetBoardImage();
        }
    }
}
