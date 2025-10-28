namespace Chess.Engine.GameModels.Pieces
{
    public class King : Piece
    {
        public override int Value => 999;
        public override char NotationCharacter => 'K';
        public override List<MovementVector> MovementVectors { get; } = PieceMovementVectors.KingMovementVectors;
        public override bool CanPieceSlide => false;

        public override Piece DeepCopy()
        {
            return new King(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private King(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }
        public King(Side side) : base(side)
        {
        }
        public King() : base(Side.White)
        {
        }
    }
}
