using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LittleFlowerBot.Repositories;

namespace LittleFlowerBot.Models.Game.Battleship
{
    public class BattleshipGame : Game
    {
        private static readonly Regex PlaceRegex = new(@"^放置\s+(航母|戰艦|巡洋艦|潛艇|驅逐艦)\s+([a-j])(10|[1-9])\s+(橫|直)$");
        private static readonly Regex AttackRegex = new(@"^([a-j])(10|[1-9])$");

        private static readonly Dictionary<string, ShipType> ShipNameMap = new()
        {
            {"航母", ShipType.Carrier},
            {"戰艦", ShipType.Battleship},
            {"巡洋艦", ShipType.Cruiser},
            {"潛艇", ShipType.Submarine},
            {"驅逐艦", ShipType.Destroyer},
        };

        private readonly IBoardGameResultsRepository _resultsRepository;

        public BattleshipGame(IBoardGameResultsRepository resultsRepository)
        {
            _resultsRepository = resultsRepository;
            GameBoard = new BattleshipBoard();
        }

        private BattleshipBoard GetBoard() => (BattleshipBoard)GameBoard;

        public override void StartGame()
        {
            Render("【海戰棋】輸入++參加遊戲（需要2位玩家）");
        }

        public override void Act(string userId, string cmd)
        {
            var board = GetBoard();

            if (cmd == "++" && board.Phase == BattleshipPhase.WaitingPlayers)
            {
                HandleJoin(userId);
                return;
            }

            if (board.Phase == BattleshipPhase.Setup)
            {
                HandleSetup(userId, cmd);
                return;
            }

            if (board.Phase == BattleshipPhase.Battle)
            {
                HandleAttack(userId, cmd);
                return;
            }
        }

        private void HandleJoin(string userId)
        {
            var board = GetBoard();

            if (board.Players.Contains(userId))
            {
                Render("你已經加入");
                return;
            }

            board.Join(userId);
            Render($"{userId} 已加入遊戲");

            if (board.IsPlayerFull())
            {
                Render("兩位玩家就位！請開始佈置船艦");
                foreach (var playerId in board.Players)
                {
                    TextRenderer.RenderPrivate(playerId, "請開始佈置你的船艦\n可用船艦：航母(5格)、戰艦(4格)、巡洋艦(3格)、潛艇(3格)、驅逐艦(2格)\n指令格式：放置 航母 a1 橫");
                }
            }
        }

        private void HandleSetup(string userId, string cmd)
        {
            var board = GetBoard();
            if (!board.Players.Contains(userId))
                return;

            var match = PlaceRegex.Match(cmd);
            if (!match.Success)
                return;

            var shipName = match.Groups[1].Value;
            var col = match.Groups[2].Value[0] - 'a';
            var row = int.Parse(match.Groups[3].Value) - 1;
            var isHorizontal = match.Groups[4].Value == "橫";
            var shipType = ShipNameMap[shipName];

            var state = board.PlayerStates[userId];

            if (state.IsSetupComplete)
            {
                TextRenderer.RenderPrivate(userId, "你已完成佈置，請等待對手");
                return;
            }

            if (state.Ships.Any(s => s.Type == shipType))
            {
                TextRenderer.RenderPrivate(userId, $"{shipName} 已放置過");
                return;
            }

            var size = GetShipSize(shipType);
            var coordinates = GenerateCoordinates(row, col, size, isHorizontal);

            if (coordinates == null)
            {
                TextRenderer.RenderPrivate(userId, "放置失敗：超出棋盤邊界");
                return;
            }

            if (coordinates.Any(c => state.OwnGrid[c.Row][c.Col] == CellState.Ship))
            {
                TextRenderer.RenderPrivate(userId, "放置失敗：與其他船艦重疊");
                return;
            }

            var ship = new Ship { Type = shipType, Coordinates = coordinates };
            state.Ships.Add(ship);
            foreach (var coord in coordinates)
            {
                state.OwnGrid[coord.Row][coord.Col] = CellState.Ship;
            }

            TextRenderer.RenderPrivate(userId, $"已放置 {shipName}（{size}格）");
            TextRenderer.RenderPrivateImage(userId, BattleshipBoardImageRenderer.RenderOwnGrid(state));
            Render($"{userId} 已放置一艘船（{state.Ships.Count}/5）");

            if (state.Ships.Count == 5)
            {
                state.IsSetupComplete = true;
                TextRenderer.RenderPrivate(userId, "佈置完成！等待對手完成佈置");

                if (board.PlayerStates.Values.All(s => s.IsSetupComplete))
                {
                    board.Phase = BattleshipPhase.Battle;
                    Render("雙方佈置完成！戰鬥開始！");
                    var firstPlayer = board.Players[board.CurrentTurnIndex];
                    Render($"請 {firstPlayer} 開始攻擊（輸入座標如 a5）");
                }
            }
        }

        private void HandleAttack(string userId, string cmd)
        {
            var board = GetBoard();
            var match = AttackRegex.Match(cmd);
            if (!match.Success)
                return;

            var currentPlayer = board.Players[board.CurrentTurnIndex];
            if (userId != currentPlayer)
            {
                Render("不是你的回合");
                return;
            }

            var col = match.Groups[1].Value[0] - 'a';
            var row = int.Parse(match.Groups[2].Value) - 1;

            var attackerState = board.PlayerStates[userId];
            if (attackerState.AttackGrid[row][col] != CellState.Unknown)
            {
                TextRenderer.RenderPrivate(userId, "這個座標已經攻擊過了");
                return;
            }

            var opponentId = board.Players.First(p => p != userId);
            var opponentState = board.PlayerStates[opponentId];

            var coordLabel = $"{(char)('a' + col)}{row + 1}";

            if (opponentState.OwnGrid[row][col] == CellState.Ship)
            {
                opponentState.OwnGrid[row][col] = CellState.Hit;
                attackerState.AttackGrid[row][col] = CellState.Hit;

                // Mark coordinate as hit on the ship object
                var hitShip = opponentState.Ships
                    .SelectMany(s => s.Coordinates, (s, c) => new { Ship = s, Coord = c })
                    .FirstOrDefault(x => x.Coord.Row == row && x.Coord.Col == col);
                if (hitShip != null)
                    hitShip.Coord.IsHit = true;

                var sunkShip = opponentState.Ships.FirstOrDefault(s => s.IsSunk && s.Coordinates.Any(c => c.Row == row && c.Col == col));
                if (sunkShip != null)
                {
                    var shipName = GetShipName(sunkShip.Type);
                    Render($"{userId} 攻擊 {coordLabel} → 命中！擊沉了{shipName}！");
                }
                else
                {
                    Render($"{userId} 攻擊 {coordLabel} → 命中！");
                }

                if (opponentState.Ships.All(s => s.IsSunk))
                {
                    board.Phase = BattleshipPhase.GameOver;
                    board.WinnerId = userId;
                    Render($"{userId} 勝利！所有敵艦已被擊沉！");
                    return;
                }
            }
            else
            {
                opponentState.OwnGrid[row][col] = CellState.Miss;
                attackerState.AttackGrid[row][col] = CellState.Miss;
                Render($"{userId} 攻擊 {coordLabel} → 未命中");
            }

            // Send updated board images to both players
            TextRenderer.RenderPrivateImage(userId, BattleshipBoardImageRenderer.RenderAttackGrid(attackerState));
            TextRenderer.RenderPrivateImage(opponentId, BattleshipBoardImageRenderer.RenderOwnGrid(opponentState));

            board.CurrentTurnIndex = (board.CurrentTurnIndex + 1) % 2;
        }

        private static string GetShipName(ShipType type)
        {
            return type switch
            {
                ShipType.Carrier => "航母",
                ShipType.Battleship => "戰艦",
                ShipType.Cruiser => "巡洋艦",
                ShipType.Submarine => "潛艇",
                ShipType.Destroyer => "驅逐艦",
                _ => "未知"
            };
        }

        private static int GetShipSize(ShipType type)
        {
            return type switch
            {
                ShipType.Carrier => 5,
                ShipType.Battleship => 4,
                ShipType.Cruiser => 3,
                ShipType.Submarine => 3,
                ShipType.Destroyer => 2,
                _ => 0
            };
        }

        private static List<Coordinate>? GenerateCoordinates(int row, int col, int size, bool isHorizontal)
        {
            var coordinates = new List<Coordinate>();
            for (int i = 0; i < size; i++)
            {
                var r = isHorizontal ? row : row + i;
                var c = isHorizontal ? col + i : col;
                if (r < 0 || r >= 10 || c < 0 || c >= 10)
                    return null;
                coordinates.Add(new Coordinate { Row = r, Col = c });
            }
            return coordinates;
        }
    }
}
