using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class Queen : Piece
    {
        public override int Value => 9;
        public override char NotationCharacter => 'Q';

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

        public override bool CanPieceSlide => true;

        public override Piece DeepCopy()
        {
            return new Queen(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private Queen(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }
        public Queen(Side side) : base(side)
        {
        }

        public Queen() : base(Side.White)
        {
        }
    }
}
