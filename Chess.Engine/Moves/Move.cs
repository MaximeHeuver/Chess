using Chess.Engine.GameModels;
using Chess.Engine.Moves.MoveSideEffects;

namespace Chess.Engine.Moves
{
    public class Move
    {
        public Square Origin { get; }
        public Square Destination { get; }
        public MoveSideEffect? SideEffect { get; }

        public Move(Square origin, Square destination, MoveSideEffect? sideEffect = null)
        {
            Origin = origin;
            Destination = destination;
            SideEffect = sideEffect;
        }
    }
}
