using Chess.Engine.GameModels;
using Chess.Engine.Moves;
using Chess.Engine.Moves.MoveSideEffects;
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
                    $"destination square {move.Destination.SquareNotation} is not among the possible moves: {string.Join(", ", possibleMoves.Select(x => x.Destination.SquareNotation))}");
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
                    var newIndex = square.BoardIndex + pieceMovementVector.Vector * iteration;

                    if (!IsBoardIndexReachable(square, pieceMovementVector.Vector, newIndex))
                    {
                        break;
                    }

                    var canPieceMoveAwayToSquare = CanPieceMoveAwayToSquare(square, board, newIndex, pieceMovementVector.CanMovementCapture);

                    var isKingInCheckAfterMove = IsKingInCheckAfterMove();

                    if (!canPieceMoveAwayToSquare || isKingInCheckAfterMove)
                    {
                        break;
                    }

                    MoveSideEffect? moveSideEffect = null;

                    if (IsMovePromotion(piece, board[newIndex]))
                    {
                        moveSideEffect = new PromotionSideEffect(new Queen(piece.Side));
                    }

                    var move = new Move(square, board[newIndex], moveSideEffect);

                    possibleMoves.Add(move);

                    if (!piece.CanPieceSlide)
                    {
                        break;
                    }

                    iteration++;
                }
            }

            if (piece is Pawn)
            {
                //check for en passant
            }
            else if (piece is King)
            {
                if (CanKingCastle(piece.Side, BoardSide.KingSide, board))
                {
                    possibleMoves.Add(new Move(
                            square,
                            board[square.BoardIndex + 2],
                            new OtherMoveSideEffect(board[square.BoardIndex + 3], board[square.BoardIndex + 1])));
                }

                if (CanKingCastle(piece.Side, BoardSide.QueenSide, board))
                {
                    possibleMoves.Add(new Move(
                        square,
                        board[square.BoardIndex - 3],
                        new OtherMoveSideEffect(board[square.BoardIndex - 4], board[square.BoardIndex - 2])));
                }
            }

            return possibleMoves;
        }

        private static bool CanKingCastle(Side side, BoardSide boardSide, List<Square> board)
        {
            if (!IsCastleOptionAvailable(side, boardSide, board))
            {
                return false;
            }

            var kingSquare = GetKingStartingSquare(side, board);

            var indexesToCheckForAttacks = boardSide == BoardSide.KingSide
                ? Enumerable.Range(0, 3)
                : Enumerable.Range(-3, 4);

            foreach (var indexToCheck in indexesToCheckForAttacks)
            {
                if (!(indexToCheck == 0 || board[kingSquare.BoardIndex + indexToCheck].Piece == null))
                {
                    return false;
                }

                if (IsSquareUnderAttackFromAnyPiece(board[kingSquare.BoardIndex + indexToCheck], board, kingSquare.Piece.Side))
                {
                    return false;
                }
            }

            return true;

            return indexesToCheckForAttacks.All(indexToCheck =>
                (indexToCheck == 0 || board[kingSquare.BoardIndex + indexToCheck].Piece == null) &&
                !IsSquareUnderAttackFromAnyPiece(board[kingSquare.BoardIndex + indexToCheck], board, kingSquare.Piece.Side));
        }

        private static Square GetKingStartingSquare(Side side, List<Square> board)
        {
            return board[side == Side.White ? 4 : 60];
        }

        private static Square GetRookStartingSquare(Side side, BoardSide boardSide, List<Square> board)
        {
            return board[(boardSide == BoardSide.KingSide ? 7 : 0) + (side == Side.White ? 0 : 56)];
        }

        private static bool IsSquareUnderAttackFromAnyPiece(Square square, List<Square> board, Side side)
        {
            return IsSquareUnderAttackFromPiece<Queen>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Rook>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Bishop>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Knight>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<King>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Pawn>(square, board, side);
        }


        private static bool IsMovePromotion(Piece piece, Square destination)
        {
            return piece is Pawn && piece!.Side == Side.White
                ? Enumerable.Range(0, 8).Contains(destination.BoardIndex)
                : Enumerable.Range(56, 8).Contains(destination.BoardIndex);
        }

        private static bool IsKingInCheckAfterMove()
        {
            // deep copy board state

            // execute move

            // check for king in check

            return false;
        }

        public static bool IsKingInCheck(Side sideOfKing, List<Square> board)
        {
            var square = GetKingSquareFromSide(sideOfKing, board);

            return IsSquareUnderAttackFromAnyPiece(square, board, sideOfKing);
        }

        private static bool IsSquareUnderAttackFromPiece<T>(Square square, IReadOnlyList<Square> board, Side side) where T : Piece, new()
        {
            var attackPieceType = new T();

            foreach (var pieceMovementVector in attackPieceType.MovementVectors.Where(x => x.CanMovementCapture))
            {
                var iteration = 1;

                while (true)
                {
                    var newIndex = square.BoardIndex + pieceMovementVector.Vector * iteration;

                    if (!IsBoardIndexReachable(square, pieceMovementVector.Vector, newIndex))
                    {
                        break;
                    }

                    var piece = board[newIndex]?.Piece;

                    if (piece is T && piece.Side != side && pieceMovementVector.CanMovementCapture)
                    {
                        return true;
                    }

                    if (piece != null || !attackPieceType.CanPieceSlide)
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
            int destinationIndex,
            bool isCaptureAllowed)
        {
            var destinationSquare = board[destinationIndex];
            var sideOfPieceToMove = squareWithPiece.Piece!.Side;

            return destinationSquare.Piece == null ||
                   isCaptureAllowed &&
                   destinationSquare.Piece.Side != sideOfPieceToMove &&
                   destinationSquare.Piece is not King;
        }

        public static bool IsCastleOptionAvailable(Side side, BoardSide boardSide, List<Square> board)
        {
            var kingSquareToCheck = GetKingStartingSquare(side, board);

            var rookSquareToCheck = GetRookStartingSquare(side, boardSide, board);

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
