using Chess.Engine.GameModels;

namespace Chess.Engine.GameModels.MoveSideEffects
{
    public class OtherMoveSideEffect : MoveSideEffect
    {
        public Square Origin { get; set; }
        public Square Destination { get; set; }

        public OtherMoveSideEffect(Square origin, Square destination)
        {
            Origin = origin;
            Destination = destination;
        }

        public override void Execute()
        {
            if (Origin.Piece == null)
            {
                throw new ArgumentException($"side effect {nameof(OtherMoveSideEffect)} not possible: {Origin.SquareNotation} must contain a piece");
            }

            Destination.Piece = Origin.Piece;
            Origin.Piece = null;
        }
    }
}
