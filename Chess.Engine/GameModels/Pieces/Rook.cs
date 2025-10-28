namespace Chess.Engine.GameModels.Pieces
{
    public class Rook : Piece
    {
        public override int Value => 5;
        public override char NotationCharacter => 'R';

        public override List<MovementVector> MovementVectors { get; } =
        [
            new MovementVector(-8, MovementCaptureOption.Both),
            new MovementVector(-1, MovementCaptureOption.Both),
            new MovementVector(8, MovementCaptureOption.Both),
            new MovementVector(1, MovementCaptureOption.Both)
        ];
        public override bool CanPieceSlide => true;

        public override Piece DeepCopy()
        {
            return new Rook(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private Rook(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }
        public Rook(Side side) : base(side)
        {
        }
    }
}
