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

        public static void ValidateAndExecuteMove(Move move, Game game)
        {
            var possibleMoves = GetPossibleMovesForPieceOnSquare(move.Origin, game);

            if (!possibleMoves.Select(x => x.Destination).Contains(move.Destination))
            {
                throw new ArgumentException(
                    $"destination square {move.Destination.SquareNotation} is not among the possible moves: {string.Join(", ", possibleMoves.Select(x => x.Destination.SquareNotation))}");
            }

            var moveFromPossibleMoves = possibleMoves.Single(x => x.Destination.Equals(move.Destination));

            ExecuteMove(moveFromPossibleMoves);
        }

        private static void ExecuteMove(Move move)
        {
            move.Destination.Piece = move.Origin.Piece;
            move.Origin.Piece = null;

            move.SideEffect?.Execute();
        }

        public static List<Move> GetPossibleMovesForPieceOnSquare(Square square, Game game)
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

                    var canPieceMoveAwayToSquare = CanPieceMoveAwayToSquare(square, game.Board, newIndex, pieceMovementVector.CanMovementCapture);


                    if (!canPieceMoveAwayToSquare)
                    {
                        break;
                    }

                    MoveSideEffect? moveSideEffect = null;

                    if (IsMovePromotion(piece, game.Board[newIndex]))
                    {
                        moveSideEffect = new PromotionSideEffect(new Queen(piece.Side));
                    }

                    var move = new Move(square, game.Board[newIndex], moveSideEffect);

                    var isKingInCheckAfterMove = IsKingInCheckAfterMove(move, game);

                    if (isKingInCheckAfterMove)
                    {
                        break;
                    }

                    possibleMoves.Add(move);

                    if (!piece.CanPieceSlide)
                    {
                        break;
                    }

                    iteration++;
                }
            }

            if (AreConditionsForEnPassantPresent(square, game))
            {
                var move = new Move(
                    square,
                    game.Board[
                        game.LastPlayedMove.Origin.BoardIndex + ((game.LastPlayedMove.Destination.BoardIndex -
                                                                  game.LastPlayedMove.Origin.BoardIndex) / 2)],
                    new CaptureSideEffect(game.LastPlayedMove.Destination)
                );

                if (!IsKingInCheckAfterMove(move, game))
                {
                    possibleMoves.Add(move);
                }
            }
            else if (piece is King)
            {
                if (CanKingCastle(piece.Side, BoardSide.KingSide, game.Board))
                {
                    possibleMoves.Add(new Move(
                            square,
                            game.Board[square.BoardIndex + 2],
                            new OtherMoveSideEffect(game.Board[square.BoardIndex + 3], game.Board[square.BoardIndex + 1])));
                }

                if (CanKingCastle(piece.Side, BoardSide.QueenSide, game.Board))
                {
                    possibleMoves.Add(new Move(
                        square,
                        game.Board[square.BoardIndex - 3],
                        new OtherMoveSideEffect(game.Board[square.BoardIndex - 4], game.Board[square.BoardIndex - 2])));
                }
            }

            return possibleMoves;
        }

        private static bool AreConditionsForEnPassantPresent(Square square, Game game)
        {
            return square.Piece is Pawn &&
                   game.LastPlayedMove?.Destination.Piece is Pawn &&
                   Math.Abs(game.LastPlayedMove.Origin.BoardIndex - game.LastPlayedMove.Destination.BoardIndex) == 16 &&
                   (square.BoardIndex == game.LastPlayedMove.Destination.BoardIndex - 1 ||
                    square.BoardIndex == game.LastPlayedMove.Destination.BoardIndex + 1);
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

            return indexesToCheckForAttacks.All(indexToCheck =>
                (indexToCheck == 0 || board[kingSquare.BoardIndex + indexToCheck].Piece == null) &&
                !IsSquareUnderAttackFromAnyPiece(board[kingSquare.BoardIndex + indexToCheck], board, kingSquare.Piece!.Side));
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

        private static bool IsKingInCheckAfterMove(Move move, Game game)
        {
            var side = move.Origin.Piece!.Side;

            var copiedGame = game.DeepCopy();

            var copiedMove = move.DeepCopy(copiedGame.Board);

            ExecuteMove(copiedMove);

            return IsKingInCheck(side, game.Board);
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

            MoveHandler.ValidateAndExecuteMove(move, game);

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
