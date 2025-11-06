using Chess.Engine.Bot.Models;
using Chess.Engine.GameModels;
using Chess.Engine.Logic;
using System.Diagnostics;

namespace Chess.Engine.Bot.Logic
{
    public class MinMaxTree
    {
        public static int ElapsedMilliseconds = 0;
        public static int NumberOfNodes = 0;
        public static int NumberOfCancelledLoops = 0;
        public static int LatestMinMaxEvaluation = 0;
        public static int MaxDepth { get; set; } = 3;
        public static PositionNode? LatestNode { get; set; }

        public static Move GetBestMoveForBlack(Game game)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();

            NumberOfCancelledLoops = 0;
            NumberOfNodes = 0;

            var root = new PositionNode(FenConverter.GameToFenNotation(game), null);

            var bestEvaluation = AlphaBeta(root, 0, -99999, 99999, false);

            var move = root.ChildPositions.First(x => x.Evaluation == bestEvaluation).Move!;

            LatestMinMaxEvaluation = bestEvaluation;

            LatestNode = root;

            stopwatch.Stop();

            ElapsedMilliseconds = stopwatch.Elapsed.Milliseconds;

            return move;
        }

        public static object lockObj = new object();

        private static int AlphaBeta(PositionNode node, int depth, decimal alpha, decimal beta, bool isMaximizingPlayer)
        {
            CancellationTokenSource cts = new();

            ParallelOptions options = new()
            {
                CancellationToken = cts.Token
            };

            var game = FenConverter.FenNotationToGame(node.FenPosition);

            if (depth == MaxDepth || IsNodeTerminal(game))
            {
                node.Evaluation = PositionEvaluator.GetPositionEvaluation(game).SummedRatings;

                return node.Evaluation;
            }

            var allPossibleMovesForAllPieceFromSide = SquareFinder.GetAllSquaresWithPiecesFromSide(game.Turn, game.Board)
                .SelectMany(square => GameStateChecker.GetPossibleMovesForPieceOnSquare(square, game)).ToList();

            node.Evaluation = isMaximizingPlayer ? int.MinValue : int.MaxValue;

            try
            {
                Parallel.ForEach(allPossibleMovesForAllPieceFromSide, options, move =>
                {
                    var copiedGame = game.DeepCopy();
                    var copiedMove = move.DeepCopy(copiedGame.Board);

                    MoveHandler.ExecuteMove(copiedMove, copiedGame);

                    var childNode = new PositionNode(FenConverter.GameToFenNotation(copiedGame), copiedMove);

                    var evaluation = AlphaBeta(childNode, depth + 1, alpha, beta, !isMaximizingPlayer);

                    Interlocked.Increment(ref NumberOfNodes);

                    node.Evaluation = isMaximizingPlayer
                        ? Math.Max(node.Evaluation, evaluation)
                        : Math.Min(node.Evaluation, evaluation);

                    node.ChildPositions.Add(childNode);

                    lock (lockObj)
                    {
                        if (isMaximizingPlayer ? alpha >= beta : beta <= alpha)
                        {
                            node.IsStoppedDueToPruning = true;
                            cts.Cancel();
                        }

                        if (isMaximizingPlayer)
                        {
                            alpha = Math.Max(alpha, node.Evaluation);
                        }
                        else
                        {
                            beta = Math.Min(beta, node.Evaluation);
                        }
                    }
                });
            }
            catch (OperationCanceledException _)
            {

            }

            return node.Evaluation;
        }

        private static bool IsNodeTerminal(Game game)
        {
            return GameStateChecker.IsStaleMate(game) || GameStateChecker.IsKingCheckmate(game);
        }
    }
}
