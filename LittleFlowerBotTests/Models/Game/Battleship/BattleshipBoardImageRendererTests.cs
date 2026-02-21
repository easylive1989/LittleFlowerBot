using LittleFlowerBot.Models.Game.Battleship;
using NUnit.Framework;
using SkiaSharp;

namespace LittleFlowerBotTests.Models.Game.Battleship;

[TestFixture]
public class BattleshipBoardImageRendererTests
{
    [Test]
    public void RenderOwnGrid_ReturnsValidPng()
    {
        var state = new PlayerState();
        state.OwnGrid[0][0] = CellState.Ship;
        state.OwnGrid[1][1] = CellState.Hit;
        state.OwnGrid[2][2] = CellState.Miss;

        var imageData = BattleshipBoardImageRenderer.RenderOwnGrid(state);

        Assert.That(imageData, Is.Not.Null);
        Assert.That(imageData.Length, Is.GreaterThan(0));

        // Verify it's a valid PNG
        using var bitmap = SKBitmap.Decode(imageData);
        Assert.That(bitmap, Is.Not.Null);
        Assert.That(bitmap.Width, Is.GreaterThan(0));
        Assert.That(bitmap.Height, Is.GreaterThan(0));
    }

    [Test]
    public void RenderAttackGrid_ReturnsValidPng()
    {
        var state = new PlayerState();
        state.AttackGrid[0][0] = CellState.Hit;
        state.AttackGrid[1][1] = CellState.Miss;

        var imageData = BattleshipBoardImageRenderer.RenderAttackGrid(state);

        Assert.That(imageData, Is.Not.Null);
        Assert.That(imageData.Length, Is.GreaterThan(0));

        using var bitmap = SKBitmap.Decode(imageData);
        Assert.That(bitmap, Is.Not.Null);
        Assert.That(bitmap.Width, Is.GreaterThan(0));
    }

    [Test]
    public void RenderOwnGrid_EmptyBoard_ReturnsValidPng()
    {
        var state = new PlayerState();

        var imageData = BattleshipBoardImageRenderer.RenderOwnGrid(state);

        Assert.That(imageData, Is.Not.Null);
        using var bitmap = SKBitmap.Decode(imageData);
        Assert.That(bitmap, Is.Not.Null);
    }

    [Test]
    public void RenderAttackGrid_EmptyBoard_ReturnsValidPng()
    {
        var state = new PlayerState();

        var imageData = BattleshipBoardImageRenderer.RenderAttackGrid(state);

        Assert.That(imageData, Is.Not.Null);
        using var bitmap = SKBitmap.Decode(imageData);
        Assert.That(bitmap, Is.Not.Null);
    }
}
