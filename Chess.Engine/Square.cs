using Chess.Engine.Pieces;

namespace Chess.Engine
{
    public class Square
    {
        public int BoardIndex { get; }
        public Piece? Piece { get; set; }

        public Square(int boardIndex)
        {
            this.BoardIndex = boardIndex;
        }

        public string PieceOccupation => Piece == null
            ? "__"
            : $"{Piece.Side.ToString().ToLower()[0]}{Piece.NotationCharacter}";

        public string SquareNotation => $"{(char)(BoardIndex % 8 + 97)}{(char)(BoardIndex / 8 + 49)}";
    }
}
