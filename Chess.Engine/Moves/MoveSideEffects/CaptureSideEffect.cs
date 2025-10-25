using Chess.Engine.GameModels;

namespace Chess.Engine.Moves.MoveSideEffects
{
    internal class CaptureSideEffect : MoveSideEffect
    {
        public Square SquareToCapture { get; }

        public CaptureSideEffect(Square squareToCapture)
        {
            SquareToCapture = squareToCapture;
        }

        public override void Execute()
        {
            if (SquareToCapture.Piece == null)
            {
                throw new ArgumentException($"SideEffectCapture failed: the square {SquareToCapture.SquareNotation} must contain a piece");
            }

            SquareToCapture.Piece = null;
        }
    }
}
