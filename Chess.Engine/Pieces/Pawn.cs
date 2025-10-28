using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class Pawn : Piece
    {
        public override int Value => 1;
        public override char NotationCharacter => 'P';

        public override List<MovementVector> MovementVectors => this.Side == Side.White
            ? [
                new MovementVector(8, MovementCaptureOption.MoveOnly),
                new MovementVector(7, MovementCaptureOption.CaptureOnly),
                new MovementVector(9, MovementCaptureOption.CaptureOnly)
            ]
            : [
                new MovementVector(-8, MovementCaptureOption.MoveOnly),
                new MovementVector(-7, MovementCaptureOption.CaptureOnly),
                new MovementVector(-9, MovementCaptureOption.CaptureOnly)
            ];

        public override bool CanPieceSlide => false;

        public override Piece DeepCopy()
        {
            return new Pawn(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private Pawn(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }
        public Pawn(Side side) : base(side)
        {
        }

        public Pawn() : base(Side.White)
        {
        }
    }
}