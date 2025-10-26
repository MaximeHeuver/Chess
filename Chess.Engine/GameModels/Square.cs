using Chess.Engine.Pieces;

namespace Chess.Engine.GameModels
{
    public class Square
    {
        public int BoardIndex { get; }
        public Piece? Piece { get; set; }

        public Square(int boardIndex)
        {
            BoardIndex = boardIndex;
        }
        private Square(int boardIndex, Piece? piece)
        {
            BoardIndex = boardIndex;
            Piece = piece;
        }

        public string PieceOccupation => Piece == null
            ? "__"
            : $"{Piece.Side.ToString().ToLower()[0]}{Piece.NotationCharacter}";

        public string SquareNotation => $"{(char)(BoardIndex % 8 + 97)}{(char)(BoardIndex / 8 + 49)}";

        public Square DeepCopy()
        {
            return new Square(BoardIndex, Piece?.DeepCopy());
        }
    }
}
