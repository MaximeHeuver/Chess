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
            MovementVectors = PieceMovementVectors.GetPawnMovementVectors(side);
        }

        public Pawn(Side side) : base(side)
        {
            MovementVectors = PieceMovementVectors.GetPawnMovementVectors(side);
        }
    }
}