using Chess.Engine.Moves;
using Chess.Engine.Pieces;
using System.Text.RegularExpressions;

namespace Chess.Engine.Handlers
{
    public static class MoveHandler
    {
        private static readonly Regex SquareNotationFormat = new Regex("^[a-h][1-8]$");

        public static void ValidateAndExecuteMove(Move move, List<Square> board)
        {
            var possibleMoves = GetPossibleMovesForPieceOnSquare(move.Origin, board);

            if (!possibleMoves.Select(x => x.Destination).Contains(move.Destination))
            {
                throw new ArgumentException(
                    $"destination square {move.Destination.SquareNotation} among the possible moves: {string.Join(", ", possibleMoves.Select(x => x.Destination.SquareNotation))}");
            }

            var sideEffect = possibleMoves.Single(x => x.Destination.Equals(move.Destination)).SideEffect;

            move.Destination.Piece = move.Origin.Piece;
            move.Origin.Piece = null;

            sideEffect?.Execute();
        }

        public static List<Move> GetPossibleMovesForPieceOnSquare(Square square, List<Square> board)
        {
            var piece = square.Piece;

            if (piece == null)
            {
                throw new ArgumentException($"the square {square.SquareNotation} does not contain a piece");
            }

            var possibleMoves = new List<Move>();

            foreach (var pieceMovementVector in piece.MovementVectors)
            {
                var iteration = 1;

                while (true)
                {
                    var newIndex = square.BoardIndex + pieceMovementVector * iteration;

                    if (!IsBoardIndexReachable(square, pieceMovementVector, newIndex))
                    {
                        break;
                    }

                    var canPieceMoveAwayToSquare = CanPieceMoveAwayToSquare(square, board, newIndex);

                    if (canPieceMoveAwayToSquare)
                    {
                        possibleMoves.Add(new Move(square, board[newIndex]));
                    }

                    if (!piece.CanPieceSlide)
                    {
                        break;
                    }

                    iteration++;
                }
            }

            return possibleMoves;
        }

        public static bool IsKingInCheck(Side sideOfKing, List<Square> board)
        {
            var square = GetKingSquareFromSide(sideOfKing, board);

            return IsSquareUnderAttackFromPiece<Queen>(square, board) ||
                   IsSquareUnderAttackFromPiece<Rook>(square, board) ||
                   IsSquareUnderAttackFromPiece<Bishop>(square, board) ||
                   IsSquareUnderAttackFromPiece<Knight>(square, board) ||
                   IsSquareUnderAttackFromPiece<Pawn>(square, board);
        }

        private static bool IsSquareUnderAttackFromPiece<T>(Square square, IReadOnlyList<Square> board) where T : Piece, new()
        {
            if (square.Piece == null)
            {
                throw new ArgumentException($"Square: {square.SquareNotation} does not contain a piece");
            }

            var attackPieceType = new T();

            foreach (var pieceMovementVector in attackPieceType.CaptureVectors)
            {
                var iteration = 1;

                while (true)
                {
                    var newIndex = square.BoardIndex + pieceMovementVector * iteration;

                    if (!IsBoardIndexReachable(square, pieceMovementVector, newIndex))
                    {
                        break;
                    }

                    var piece = board[newIndex]?.Piece;

                    if (piece is T && piece.Side != square.Piece.Side)
                    {
                        return true;
                    }

                    if (piece != null || attackPieceType is Knight)
                    {
                        break;
                    }

                    iteration++;
                }
            }

            return false;
        }

        private static Square GetKingSquareFromSide(Side sideOfKing, IEnumerable<Square> board)
        {
            var kingSquare = board.SingleOrDefault(x =>
                x.Piece is King &&
                x.Piece.Side == sideOfKing);

            if (kingSquare == null)
            {
                throw new ArgumentException($"Unable to find a single king for side: {sideOfKing}");
            }

            return kingSquare;
        }

        private static bool IsBoardIndexReachable(
            Square origin,
            int pieceMovementVector,
            int destinationIndex)
        {
            if (destinationIndex is > 63 or < 0)
            {
                return false;
            }

            var horizontalSquareDifferenceOfMovementVector = (pieceMovementVector % 8 + 8) % 8 - 4;
            var horizontalSquareDifferenceOfNewIndex = origin.BoardIndex % 8 - destinationIndex % 8;

            return horizontalSquareDifferenceOfMovementVector * horizontalSquareDifferenceOfNewIndex >= 0;
        }

        public static bool CanPieceMoveAwayToSquare(
            Square squareWithPiece,
            IReadOnlyList<Square> board,
            int destinationIndex)
        {
            var destinationSquare = board[destinationIndex];
            var sideOfPieceToMove = squareWithPiece.Piece!.Side;

            return destinationSquare.Piece == null ||
                   destinationSquare.Piece.Side != sideOfPieceToMove &&
                   destinationSquare.Piece is not King;
        }

        public static bool IsCastleOptionAvailable(Side side, BoardSide boardSide, List<Square> board)
        {
            var kingSquareToCheck = board[side == Side.White ? 4 : 60];

            var rookSquareToCheck = board[(boardSide == BoardSide.KingSide ? 7 : 0) + (side == Side.White ? 0 : 56)];

            return kingSquareToCheck.Piece is King &&
                   !kingSquareToCheck.Piece.HasPieceMoved &&
                   rookSquareToCheck.Piece is Rook &&
                   !rookSquareToCheck.Piece.HasPieceMoved;
        }

        public static void MovePiece(Game game, string originSquareAbbreviation, string destinationSquareAbbreviation)
        {
            var originSquare = GetSquareFromAbbreviation(game.Board, originSquareAbbreviation);
            var destinationSquare = GetSquareFromAbbreviation(game.Board, destinationSquareAbbreviation);

            if (originSquare.Piece?.Side != game.Turn)
            {
                throw new ArgumentException($"it is currently not the turn of {originSquare.Piece?.Side.ToString().ToLower()}");
            }

            var move = new Move(originSquare, destinationSquare);

            MoveHandler.ValidateAndExecuteMove(move, game.Board);

            game.LastPlayedMove = move;

            game.Turn = game.Turn == Side.White ? Side.Black : Side.White;
        }

        public static Square GetSquareFromAbbreviation(List<Square> board, string abbreviation)
        {
            if (!SquareNotationFormat.IsMatch(abbreviation))
            {
                throw new ArgumentException($"{abbreviation} is not a valid square name");
            }

            var index = (abbreviation[0] - 97) + (abbreviation[1] - 49) * 8;

            return board[index];
        }
    }
}
