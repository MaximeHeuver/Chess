using Chess.Engine.Bot.Logic;
using Chess.Engine.GameModels;
using Chess.Engine.Logic;

namespace Chess.WebApp.Pages
{
    public partial class ChessBoard
    {
        public Game? Game { get; private set; } = null;
        public List<int>? HighlightIndexes { get; private set; } = [];
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

                SetHighlightSquareIndexes(squareIndex);

                return;
            }

            if (LastSelectedPieceIndex == -1)
            {
                return;
            }

            try
            {
                var move = MoveHandler.GetMoveFromIndexes(Game, LastSelectedPieceIndex, squareIndex);

                MoveHandler.ValidateAndExecuteMove(move, Game);

                LastSelectedPieceIndex = -1;

                HighlightIndexes = [];

                var botMove = MinMaxTree.GetBestMoveForBlack(Game);

                var botMoveInThisGame = botMove.DeepCopy(Game.Board);

                MoveHandler.ValidateAndExecuteMove(botMoveInThisGame, Game);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

                LastErrorMessage = ex.Message;

                LastSelectedPieceIndex = -1;

                HighlightIndexes = [];
            }
        }

        private void SetHighlightSquareIndexes(int squareIndex)
        {
            var square = Game.Board[squareIndex];

            var possibleMoves = GameStateChecker.GetPossibleMovesForPieceOnSquare(square, Game);

            HighlightIndexes = possibleMoves.Select(x => x.Destination.BoardIndex).ToList();
        }
    }
}
