using Chess.Engine.GameModels;
using Chess.Engine.GameModels.MoveSideEffects;
using Chess.Engine.GameModels.Pieces;

namespace Chess.Engine.Logic
{
    public class GameStateChecker
    {

        public static bool IsCastleOptionAvailable(Side side, BoardSide boardSide, List<Square> board)
        {
            var kingSquareToCheck = SquareFinder.GetKingStartingSquare(side, board);

            var rookSquareToCheck = SquareFinder.GetRookStartingSquare(side, boardSide, board);

            return kingSquareToCheck.Piece is King &&
                   !kingSquareToCheck.Piece.HasPieceMoved &&
                   rookSquareToCheck.Piece is Rook &&
                   !rookSquareToCheck.Piece.HasPieceMoved;
        }
        public static bool IsKingInCheck(Game game, Side side)
        {
            var square = SquareFinder.GetKingSquareFromSide(side, game.Board);

            return IsSquareUnderAttackFromAnyPiece(square, game.Board, side);
        }

        public static bool IsKingInCheck(Game game)
        {
            var square = SquareFinder.GetKingSquareFromSide(game.Turn, game.Board);

            return IsSquareUnderAttackFromAnyPiece(square, game.Board, game.Turn);
        }

        public static bool IsKingCheckmate(Game game)
        {
            if (!IsKingInCheck(game))
            {
                return false;
            }

            var allPiecesFromSide = SquareFinder.GetAllSquaresWithPiecesFromSide(game.Turn, game.Board);

            var aaa = allPiecesFromSide.All(x => GetPossibleMovesForPieceOnSquare(x, game).Count == 0);
            return aaa;
        }

        public static bool IsSquareUnderAttackFromAnyPiece(Square square, List<Square> board, Side side)
        {
            return IsSquareUnderAttackFromPiece<Queen>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Rook>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Bishop>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Knight>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<King>(square, board, side) ||
                   IsSquareUnderAttackFromPiece<Pawn>(square, board, side);
        }

        private static bool IsSquareUnderAttackFromPiece<T>(Square square, IReadOnlyList<Square> board, Side side)
            where T : Piece
        {
            Piece attackPieceType =
                (T)Activator.CreateInstance(typeof(T), [side == Side.White ? Side.Black : Side.White])!;

            foreach (var pieceMovementVector in attackPieceType.MovementVectors.Where(x =>
                         x.MovementCaptureOption is MovementCaptureOption.Both or MovementCaptureOption.CaptureOnly))
            {
                var iteration = 1;

                while (true)
                {
                    var oppositeVector = pieceMovementVector.Vector * -1;

                    var newIndex = square.BoardIndex + oppositeVector * iteration;

                    if (!IsBoardIndexReachable(square, oppositeVector, newIndex))
                    {
                        break;
                    }

                    var piece = board[newIndex]?.Piece;

                    if (piece is T && piece.Side != side)
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

        public static bool IsStaleMate(Game game)
        {
            var numberOfOccurrencesOfLatestPosition = game.allPositionsSinceLastCapture.Count(x =>
                x.Split(' ')[0] == game.allPositionsSinceLastCapture.Last().Split()[0]);

            if (numberOfOccurrencesOfLatestPosition == 3)
            {
                return true;
            }

            var allSquaresWithPieces = game.Board.Where(x => x.Piece != null).ToList();

            var areOnlyKingsInPlay = allSquaresWithPieces.Count == 2 &&
                                     allSquaresWithPieces.Exists(x => x.Piece is King && x.Piece.Side == Side.White) &&
                                     allSquaresWithPieces.Exists(x => x.Piece is King && x.Piece.Side == Side.Black);

            var areOnlyKingsInPlayWithSingleMinorPiece = allSquaresWithPieces.Count == 3 &&
                                                         allSquaresWithPieces.Exists(x =>
                                                             x.Piece is King && x.Piece.Side == Side.White) &&
                                                         allSquaresWithPieces.Exists(x =>
                                                             x.Piece is King && x.Piece.Side == Side.Black) &&
                                                         allSquaresWithPieces.Exists(x => x.Piece is Bishop or Knight);

            if (areOnlyKingsInPlay || areOnlyKingsInPlayWithSingleMinorPiece)
            {
                return true;
            }

            var allSquaresWithPiecesFromSideAtPlay =
                game.Board.Where(x => x.Piece != null && x.Piece.Side == game.Turn).ToList();

            var allPossibleMoves = allSquaresWithPiecesFromSideAtPlay
                .SelectMany(x => GetPossibleMovesForPieceOnSquare(x, game)).ToList();

            return allPossibleMoves.Count == 0 && !IsKingInCheck(game);
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

                    var canPieceMoveAwayToSquare = CanPieceMoveAwayToSquare(square, game.Board, newIndex,
                        pieceMovementVector.MovementCaptureOption);


                    if (!canPieceMoveAwayToSquare)
                    {
                        break;
                    }

                    MoveSideEffect? moveSideEffect = null;

                    if (IsMovePromotion(piece, game.Board[newIndex]))
                    {
                        moveSideEffect = new PromotionSideEffect(game.Board[newIndex]);
                    }

                    var move = new Move(square, game.Board[newIndex], moveSideEffect);

                    if (!IsKingInCheckAfterMove(move, game))
                    {
                        possibleMoves.Add(move);
                    }

                    if (!piece.CanPieceSlide || game.Board[newIndex].Piece != null)
                    {
                        break;
                    }

                    iteration++;
                }
            }

            if (AreConditionsForDoublePawnStepPresent(square, game))
            {
                var move = new Move(
                    square,
                    game.Board[
                        square.BoardIndex + square.Piece!.MovementVectors
                            .Single(x => x.MovementCaptureOption == MovementCaptureOption.MoveOnly).Vector * 2]);

                if (!IsKingInCheckAfterMove(move, game))
                {
                    possibleMoves.Add(move);
                }
            }
            else if (AreConditionsForEnPassantPresent(square, game))
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
                        game.Board[square.BoardIndex - 2],
                        new OtherMoveSideEffect(game.Board[square.BoardIndex - 4], game.Board[square.BoardIndex - 1])));
                }
            }

            return possibleMoves;
        }

        private static bool IsKingInCheckAfterMove(Move move, Game game)
        {
            var side = move.Origin.Piece!.Side;

            var copiedGame = game.DeepCopy();

            var copiedMove = move.DeepCopy(copiedGame.Board);

            MoveHandler.ExecuteMove(copiedMove, copiedGame);

            return IsKingInCheck(copiedGame, side);
        }

        private static bool AreConditionsForDoublePawnStepPresent(Square square, Game game)
        {
            if (square.Piece is not Pawn)
            {
                return false;
            }

            return square.Piece.Side == Side.White ? square.BoardIndex / 8 == 1 : square.BoardIndex / 8 == 6 &&
                   game.Board[square.BoardIndex + square.Piece.MovementVectors.Single(x => x.MovementCaptureOption == MovementCaptureOption.MoveOnly).Vector].Piece == null &&
                   CanPieceMoveAwayToSquare(square, game.Board, square.BoardIndex + (square.Piece.MovementVectors.Single(x => x.MovementCaptureOption == MovementCaptureOption.MoveOnly).Vector * 2), MovementCaptureOption.MoveOnly);
        }

        private static bool AreConditionsForEnPassantPresent(Square square, Game game)
        {
            return square.Piece is Pawn &&
                   game.LastPlayedMove?.Destination.Piece is Pawn &&
                   Math.Abs(game.LastPlayedMove.Origin.BoardIndex - game.LastPlayedMove.Destination.BoardIndex) == 16 &&
                   (square.BoardIndex == game.LastPlayedMove.Destination.BoardIndex - 1 ||
                    square.BoardIndex == game.LastPlayedMove.Destination.BoardIndex + 1);
        }

        private static bool IsMovePromotion(Piece piece, Square destination)
        {
            return piece is Pawn && (piece!.Side == Side.White
                ? Enumerable.Range(56, 8).Contains(destination.BoardIndex)
                : Enumerable.Range(0, 8).Contains(destination.BoardIndex));
        }

        private static bool IsBoardIndexReachable(Square origin, int pieceMovementVector, int destinationIndex)
        {
            if (destinationIndex is > 63 or < 0)
            {
                return false;
            }

            var previouslyCheckedSquareIndex = destinationIndex - pieceMovementVector;

            var horizontalSquareDifferenceOfMovementVector = (pieceMovementVector % 8 + 8) % 8 - 4;
            var horizontalSquareDifferenceOfNewIndex = previouslyCheckedSquareIndex % 8 - destinationIndex % 8;

            return horizontalSquareDifferenceOfMovementVector * horizontalSquareDifferenceOfNewIndex >= 0;
        }

        public static bool CanPieceMoveAwayToSquare(Square squareWithPiece, IReadOnlyList<Square> board, int destinationIndex, MovementCaptureOption movementCaptureOption)
        {
            var destinationSquare = board[destinationIndex];
            var sideOfPieceToMove = squareWithPiece.Piece!.Side;

            return (destinationSquare.Piece == null &&
                    movementCaptureOption is MovementCaptureOption.MoveOnly or MovementCaptureOption.Both) ||
                   (destinationSquare.Piece != null &&
                    movementCaptureOption is MovementCaptureOption.CaptureOnly or MovementCaptureOption.Both &&
                    destinationSquare.Piece.Side != sideOfPieceToMove &&
                    destinationSquare.Piece is not King);
        }

        private static bool CanKingCastle(Side side, BoardSide boardSide, List<Square> board)
        {
            if (!GameStateChecker.IsCastleOptionAvailable(side, boardSide, board))
            {
                return false;
            }

            var kingSquare = SquareFinder.GetKingStartingSquare(side, board);

            var indexesToCheckForAttacks = boardSide == BoardSide.KingSide
                ? Enumerable.Range(0, 3)
                : Enumerable.Range(-3, 4);

            return indexesToCheckForAttacks.All(indexToCheck =>
                (indexToCheck == 0 || board[kingSquare.BoardIndex + indexToCheck].Piece == null) &&
                !GameStateChecker.IsSquareUnderAttackFromAnyPiece(board[kingSquare.BoardIndex + indexToCheck], board, kingSquare.Piece!.Side));
        }

    }
}
