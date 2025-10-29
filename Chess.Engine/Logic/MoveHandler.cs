

using Chess.Engine.GameModels;
using Chess.Engine.GameModels.MoveSideEffects;

namespace Chess.Engine.Logic
{
    public static class MoveHandler
    {
        public static void ValidateAndExecuteMove(Move move, Game game)
        {
            if (move.Origin.Piece == null)
            {
                throw new ArgumentException($"{move.Origin.SquareNotation} does not contain a piece");
            }

            if (move.Origin.Piece?.Side != game.Turn)
            {
                throw new ArgumentException($"it is currently not the turn of {move.Origin.Piece?.Side.ToString().ToLower()}");
            }

            var possibleMoves = GameStateChecker.GetPossibleMovesForPieceOnSquare(move.Origin, game);

            if (!possibleMoves.Select(x => x.Destination).Contains(move.Destination))
            {
                throw new ArgumentException(
                    $"destination square {move.Destination.SquareNotation} is not among the possible moves: {string.Join(", ", possibleMoves.Select(x => x.Destination.SquareNotation))}");
            }

            var moveFromPossibleMoves = possibleMoves.Single(x => x.Destination.Equals(move.Destination));

            ExecuteMove(moveFromPossibleMoves, game);
        }

        public static void ExecuteMove(Move move, Game game)
        {
            if (move.Destination.Piece != null || move.SideEffect is CaptureSideEffect)
            {
                game.ClearPositionsList();
            }

            move.Destination.Piece = move.Origin.Piece;
            move.Origin.Piece = null;

            move.SideEffect?.Execute();

            move.Destination.Piece!.HasPieceMoved = true;

            game.AddPosition(FenConverter.GameToFenNotation(game));

            game.LastPlayedMove = move;

            game.Turn = game.Turn == Side.White ? Side.Black : Side.White;
        }

        public static Move GetMoveFromIndexes(Game game, int originSquareIndex, int destinationSquareIndex)
        {
            return new Move(game.Board[originSquareIndex], game.Board[destinationSquareIndex]);
        }

        public static Move GetMoveFromSquareNotation(Game game, string origin, string destination)
        {
            return new Move(ChessNotationConverter.GetSquareFromAbbreviation(game.Board, origin), ChessNotationConverter.GetSquareFromAbbreviation(game.Board, destination));
        }
    }
}
