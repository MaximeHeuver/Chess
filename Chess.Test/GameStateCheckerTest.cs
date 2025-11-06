using Chess.Engine.GameModels;
using Chess.Engine.GameModels.Pieces;
using Chess.Engine.Logic;

namespace Chess.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetAttackersAndDefenders_TwoAttackersThreeDefenders_CorrectAmount()
        {
            const string fenNotation = "k3r3/8/8/8/2n2p2/4P3/8/K3Q1B1 w - - 0 1";

            var game = FenConverter.FenNotationToGame(fenNotation);

            var square = game.Board.Single(x => x.Piece is Pawn && x.Piece.Side == Side.White);

            var amountOfDefenders = GameStateChecker.GetAmountOfAttackersToSquare(square, game.Board, Side.Black);
            var amountOfAttackers = GameStateChecker.GetAmountOfAttackersToSquare(square, game.Board, Side.White);

            Assert.That(amountOfDefenders, Is.EqualTo(2));
            Assert.That(amountOfAttackers, Is.EqualTo(3));
        }
    }
}