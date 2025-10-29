using Chess.Engine.Bot.Models;
using Chess.Engine.GameModels;
using Chess.Engine.Logic;

namespace Chess.Engine.Bot.Logic
{
    public class MinMaxTree
    {
        private const int MaxDepth = 2;

        public static void AAA(Game game)
        {
            var root = new PositionNode(FenConverter.GameToFenNotation(game), PositionEvaluator.GetPositionEvaluation(game));

            Recurse(root, 1);
        }

        private static void Recurse(PositionNode node, int iteration)
        {
            if (iteration == MaxDepth)
            {
                return;
            }

            var game = FenConverter.FenNotationToGame(node.FenPosition);

            var allPossibleMovesForAllPieceFromSide = SquareFinder.GetAllSquaresWithPiecesFromSide(game.Turn, game.Board)
                .SelectMany(square => GameStateChecker.GetPossibleMovesForPieceOnSquare(square, game));

            foreach (var move in allPossibleMovesForAllPieceFromSide)
            {
                var copiedGame = game.DeepCopy();
                var copiedMove = move.DeepCopy(copiedGame.Board);

                MoveHandler.ExecuteMove(copiedMove, copiedGame);

                var positionNode = new PositionNode(
                    FenConverter.GameToFenNotation(copiedGame),
                    PositionEvaluator.GetPositionEvaluation(copiedGame));

                Recurse(positionNode, iteration + 1);
            }
        }
    }
}
