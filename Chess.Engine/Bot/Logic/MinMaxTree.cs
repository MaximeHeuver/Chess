using Chess.Engine.Bot.Models;
using Chess.Engine.GameModels;
using Chess.Engine.Logic;

namespace Chess.Engine.Bot.Logic
{
    public class MinMaxTree
    {
        private const int MaxDepth = 2;

        public static Move GetBestMoveForBlack(Game game)
        {
            var root = new PositionNode(FenConverter.GameToFenNotation(game), PositionEvaluator.GetPositionEvaluation(game).BlackRating, true, null);

            Recurse(root, 1);

            return root.ChildPositions.First(x => x.Evaluation == root.MinMaxedEvaluation).Move!;
        }

        private static void Recurse(PositionNode node, int iteration)
        {
            if (iteration == MaxDepth)
            {
                return;
            }

            var game = FenConverter.FenNotationToGame(node.FenPosition);

            var allPossibleMovesForAllPieceFromSide = SquareFinder.GetAllSquaresWithPiecesFromSide(game.Turn, game.Board)
                .SelectMany(square => GameStateChecker.GetPossibleMovesForPieceOnSquare(square, game)).ToList();

            foreach (var move in allPossibleMovesForAllPieceFromSide)
            {
                var copiedGame = game.DeepCopy();
                var copiedMove = move.DeepCopy(copiedGame.Board);

                MoveHandler.ExecuteMove(copiedMove, copiedGame);

                var childNode = new PositionNode(
                    FenConverter.GameToFenNotation(copiedGame),
                    PositionEvaluator.GetPositionEvaluation(copiedGame).WhiteToBlackRatio.blackFraction,
                    iteration % 2 == 0,
                    iteration == 1 ? copiedMove : null); //todo convert move to string for efficient storage

                node.ChildPositions.Add(childNode);

                Recurse(childNode, iteration + 1);
            }
        }
    }
}
