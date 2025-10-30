namespace Chess.Engine.GameModels.Pieces
{
    public class King : Piece
    {
        public override int Value => 20;
        public override char NotationCharacter => 'K';
        public override List<MovementVector> MovementVectors { get; } =
        [
            new MovementVector(-9, MovementCaptureOption.Both),
            new MovementVector(-8, MovementCaptureOption.Both),
            new MovementVector(-7, MovementCaptureOption.Both),
            new MovementVector(-1, MovementCaptureOption.Both),
            new MovementVector(1, MovementCaptureOption.Both),
            new MovementVector(7, MovementCaptureOption.Both),
            new MovementVector(8, MovementCaptureOption.Both),
            new MovementVector(9, MovementCaptureOption.Both)
        ];
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
