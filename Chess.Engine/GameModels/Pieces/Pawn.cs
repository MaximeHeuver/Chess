namespace Chess.Engine.GameModels.Pieces
{
    public class Pawn : Piece
    {
        public override int Value => 1;
        public override char NotationCharacter => 'P';

        public override List<MovementVector> MovementVectors { get; } = [];

        public override bool CanPieceSlide => false;

        public override Piece DeepCopy()
        {
            return new Pawn(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private Pawn(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
            MovementVectors = DetermineMovementVectors(side);
        }

        public Pawn(Side side) : base(side)
        {
            MovementVectors = DetermineMovementVectors(side);
        }

        private static List<MovementVector> DetermineMovementVectors(Side side)
        {
            return side == Side.White
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
        }
    }
}