namespace Chess.Engine.Pieces
{
    public abstract class Piece
    {
        public Side Side { get; }
        public abstract int Value { get; }
        public abstract char NotationCharacter { get; }
        public abstract int[] MovementVectors { get; }
        public abstract bool CanPieceSlide { get; }
        public abstract int[] CaptureVectors { get; }

        public bool HasPieceMoved { get; } = false;

        protected Piece(Side side)
        {
            Side = side;
        }
    }
}
