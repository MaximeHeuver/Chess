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

        public Move DeepCopy(List<Square> copiedBoard)
        {
            //todo maybe copy side effect as well, but I don't think that that's even relevant
            return new Move(copiedBoard[Origin.BoardIndex], copiedBoard[Destination.BoardIndex]);
        }
    }
}
