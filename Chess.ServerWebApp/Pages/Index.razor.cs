using Chess.Engine.Bot.Logic;
using Chess.Engine.GameModels;
using Chess.Engine.Logic;
using Microsoft.AspNetCore.Components;

namespace Chess.ServerWebApp.Pages
{
    public partial class Index
    {
        public Game? Game { get; private set; } = null;
        public List<int>? HighlightIndexes { get; private set; } = [];
        public string? LastErrorMessage { get; set; }
        public int LastSelectedPieceIndex { get; set; } = -1;
        public Side SideToView { get; set; } = Side.White;

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

                Task.Run(ExecuteBotMove);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

                LastErrorMessage = ex.Message;

                LastSelectedPieceIndex = -1;

                HighlightIndexes = [];
            }
        }

        private async Task ExecuteBotMove()
        {
            var botMove = MinMaxTree.GetBestMoveForBlack(Game);

            var botMoveInThisGame = botMove.DeepCopy(Game.Board);

            MoveHandler.ValidateAndExecuteMove(botMoveInThisGame, Game);

            await InvokeAsync(() => StateHasChanged());
        }

        private void SetHighlightSquareIndexes(int squareIndex)
        {
            var square = Game.Board[squareIndex];

            var possibleMoves = GameStateChecker.GetPossibleMovesForPieceOnSquare(square, Game);

            HighlightIndexes = possibleMoves.Select(x => x.Destination.BoardIndex).ToList();
        }

        private void HandleSliderChange(ChangeEventArgs changeEventArgs, string nameOfValueToUpdate)
        {
            if (!int.TryParse(((string)changeEventArgs.Value), out var value)) return;

            Console.WriteLine($"setting {nameOfValueToUpdate} to {value}");

            switch (nameOfValueToUpdate)
            {
                case "MaxDepth":
                    MinMaxTree.MaxDepth = value;
                    break;
                case "KingInCheckScoreModifier":
                    PositionEvaluator.KingInCheckScoreWeight = value;
                    break;
                case "PieceMovementOptionsScoreModifier":
                    PositionEvaluator.PieceMovementOptionsScoreWeight = value;
                    break;
                case "PieceValueScoreModifier":
                    PositionEvaluator.PieceValueScoreWeight = value;
                    break;
                default:
                    throw new ArgumentException($"nameOfValueToUpdate: {nameOfValueToUpdate} is not known");
            }
        }

        private void SwitchSideToView()
        {
            SideToView = SideToView == Side.White ? Side.Black : Side.White;

            StateHasChanged();
        }
    }
}
