using Chess.Engine.GameModels;
using Chess.Engine.Handlers;

namespace Chess.WebApp.Pages
{
    public partial class ChessBoard
    {
        public Game? Game { get; private set; } = null;
        public string? LastErrorMessage { get; set; }
        public int LastSelectedPieceIndex { get; set; } = -1;

        protected override void OnInitialized()
        {
            Game = new Game();

            Game.InitializeStandardGame();

            base.OnInitialized();
        }

        private void OnSquareClick(int squareIndex)
        {
            if (LastSelectedPieceIndex == -1 && Game?.Board[squareIndex].Piece != null && Game?.Board[squareIndex].Piece!.Side == Game.Turn)
            {
                LastSelectedPieceIndex = squareIndex;
                return;
            }

            if (LastSelectedPieceIndex == -1)
            {
                return;
            }

            try
            {
                MoveHandler.MovePiece(Game, LastSelectedPieceIndex, squareIndex);

                LastSelectedPieceIndex = -1;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
                LastErrorMessage = ex.Message;
                LastSelectedPieceIndex = -1;
            }
        }
    }
}
