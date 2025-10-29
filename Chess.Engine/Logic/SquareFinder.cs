using Chess.Engine.GameModels;
using Chess.Engine.GameModels.Pieces;

namespace Chess.Engine.Logic
{
    public class SquareFinder
    {
        public static Square GetKingStartingSquare(Side side, List<Square> board)
        {
            return board[side == Side.White ? 4 : 60];
        }

        public static Square GetRookStartingSquare(Side side, BoardSide boardSide, List<Square> board)
        {
            return board[(boardSide == BoardSide.KingSide ? 7 : 0) + (side == Side.White ? 0 : 56)];
        }

        public static Square GetKingSquareFromSide(Side sideOfKing, IEnumerable<Square> board)
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

        public static List<Square> GetAllSquaresWithPiecesFromSide(Side side, List<Square> board)
        {
            return board.Where(x => x.Piece != null && x.Piece.Side == side).ToList();
        }
    }
}
