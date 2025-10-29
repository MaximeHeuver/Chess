using Chess.Engine.Bot.Models;
using Chess.Engine.GameModels;
using Chess.Engine.Logic;

namespace Chess.Engine.Bot.Logic
{
    public class PositionEvaluator
    {
        private const int PieceValueScoreWeight = 5;
        private const int PieceMovementOptionsScoreWeight = 2;
        private const int PieceCaptureOptionsScoreWeight = 2;

        public static PositionEvaluation GetPositionEvaluation(Game game)
        {
            var whiteEvaluation = GetEvaluationScore(game, Side.White);
            var blackEvaluation = GetEvaluationScore(game, Side.Black);

            return new PositionEvaluation(whiteEvaluation, blackEvaluation);
        }

        private static decimal GetEvaluationScore(Game game, Side side)
        {
            var allSquaresWithPiecesFromSide = SquareFinder.GetAllSquaresWithPiecesFromSide(side, game.Board);

            var pieceValues = allSquaresWithPiecesFromSide.Sum(square => square.Piece!.Value);

            var allMovementSquares = allSquaresWithPiecesFromSide.Sum(square => GameStateChecker.GetPossibleMovesForPieceOnSquare(square, game).Count);

            return pieceValues * PieceValueScoreWeight +
                   allMovementSquares * PieceMovementOptionsScoreWeight;
        }
    }
}
