using Chess.Engine.Bot.Models;
using Chess.Engine.GameModels;
using Chess.Engine.Logic;

namespace Chess.Engine.Bot.Logic
{
    public class PositionEvaluator
    {
        public static int PieceValueScoreWeight { get; set; } = 10;
        public static int PieceMovementOptionsScoreWeight { get; set; } = 5;
        public static int KingInCheckScoreWeight { get; set; } = 1;
        public static int CenterControlScoreWeight { get; set; } = 5;
        public static int AttackedScoreWeight { get; set; } = 40;

        private const double CenterX = 3.5;
        private const double CenterY = 3.5;


        public static PositionEvaluation GetPositionEvaluation(Game game)
        {
            var whiteEvaluation = GetEvaluationScore(game, Side.White);
            var blackEvaluation = GetEvaluationScore(game, Side.Black);

            return new PositionEvaluation(whiteEvaluation, blackEvaluation);
        }

        private static int GetEvaluationScore(Game game, Side side)
        {
            var allSquaresWithPiecesFromSide = SquareFinder.GetAllSquaresWithPiecesFromSide(side, game.Board);

            var pieceValues = allSquaresWithPiecesFromSide.Sum(square => square.Piece!.Value) * PieceValueScoreWeight;

            var allPossibleMoves = allSquaresWithPiecesFromSide
                .SelectMany(square => GameStateChecker.GetPossibleMovesForPieceOnSquare(square, game)).ToList();

            var totalAmountOfReachableSquares = allPossibleMoves.Count() * PieceMovementOptionsScoreWeight;

            var centerControlScore = GetCenterControlScore(allPossibleMoves) * CenterControlScoreWeight;

            var attackedScore = 0;

            foreach (var square in allSquaresWithPiecesFromSide)
            {
                var amountOfAttackers = GameStateChecker.GetAmountOfAttackersToSquare(square, game.Board, side == Side.White ? Side.Black : Side.White);
                var amountOfDefenders = GameStateChecker.GetAmountOfAttackersToSquare(square, game.Board, side);

                attackedScore += amountOfDefenders - amountOfAttackers;
            }

            attackedScore *= AttackedScoreWeight;

            var ownKingInCheckScore = GameStateChecker.IsKingInCheck(game, side) ? -KingInCheckScoreWeight : 0;

            var enemyKingInCheckScore = GameStateChecker.IsKingInCheck(game, side == Side.White ? Side.Black : Side.White) ? KingInCheckScoreWeight : 0;

            var checkMateScore = GameStateChecker.IsKingCheckmate(game)
                ? game.Turn == side ? -99999 : 99999
                : 0;

            // var centerControlScore = GetCenterControlScore(allPossibleMoves);

            //checkmate

            //stalemate

            //captures

            return pieceValues +
                   totalAmountOfReachableSquares +
                   attackedScore +
                   ownKingInCheckScore +
                   enemyKingInCheckScore +
                   checkMateScore +
                   centerControlScore;
        }

        private static int GetCenterControlScore(List<Move> allPossibleMoves)
        {
            return allPossibleMoves.Sum(move =>
            {
                int x = move.Destination.BoardIndex % 8;
                int y = move.Destination.BoardIndex / 8;

                double distance = Math.Sqrt(Math.Pow(x - CenterX, 2) + Math.Pow(y - CenterY, 2));

                double maxDistance = Math.Sqrt(CenterX * CenterX + CenterY * CenterY);

                int score = (int)Math.Round(6 * (1 - (distance / maxDistance)));

                return Math.Max(0, Math.Min(6, score));
            });
        }
    }
}
