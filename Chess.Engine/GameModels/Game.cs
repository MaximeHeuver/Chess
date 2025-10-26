using Chess.Engine.Moves;
using Chess.Engine.Pieces;

namespace Chess.Engine.GameModels
{
    public class Game
    {
        public Side Turn { get; set; } = Side.White;
        public List<Square> Board { get; } = Enumerable.Range(0, 64).Select(index => new Square(index)).ToList();
        public Move? LastPlayedMove { get; set; } = null;

        public Game()
        {

        }

        private Game(List<Square> board, Side turn, Move? lastPlayedMove)
        {
            this.Board = board;
            this.Turn = turn;
            this.LastPlayedMove = lastPlayedMove;
        }

        public void InitializeStandardGame()
        {
            foreach (var i in Enumerable.Range(8, 8))
            {
                Board[i].Piece = new Pawn(Side.White);
            }

            foreach (var i in Enumerable.Range(48, 8))
            {
                Board[i].Piece = new Pawn(Side.Black);
            }

            Board[0].Piece = new Rook(Side.White);
            Board[1].Piece = new Knight(Side.White);
            Board[2].Piece = new Bishop(Side.White);
            Board[3].Piece = new Queen(Side.White);
            Board[4].Piece = new King(Side.White);
            Board[5].Piece = new Bishop(Side.White);
            Board[6].Piece = new Knight(Side.White);
            Board[7].Piece = new Rook(Side.White);

            Board[56].Piece = new Rook(Side.Black);
            Board[57].Piece = new Knight(Side.Black);
            Board[58].Piece = new Bishop(Side.Black);
            Board[59].Piece = new Queen(Side.Black);
            Board[60].Piece = new King(Side.Black);
            Board[61].Piece = new Bishop(Side.Black);
            Board[62].Piece = new Knight(Side.Black);
            Board[63].Piece = new Rook(Side.Black);
        }

        public Game DeepCopy()
        {
            var copiedBoard = Board.Select(x => x.DeepCopy()).ToList();

            return new Game(copiedBoard, Turn == Side.White ? Side.White : Side.Black, LastPlayedMove?.DeepCopy(copiedBoard));
        }
    }
}
