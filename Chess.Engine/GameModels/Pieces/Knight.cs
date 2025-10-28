namespace Chess.Engine.GameModels.Pieces
{
    public class Knight : Piece
    {
        public override int Value => 3;
        public override char NotationCharacter => 'N';

        public override List<MovementVector> MovementVectors { get; } = PieceMovementVectors.KnightMovementVectors;

        public override bool CanPieceSlide => false;

        public override Piece DeepCopy()
        {
            return new Knight(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private Knight(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }
        public Knight(Side side) : base(side)
        {
        }
        public Knight() : base(Side.White)
        {
        }
    }
}
