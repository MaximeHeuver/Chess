namespace Chess.Engine.GameModels.Pieces
{
    public class Rook : Piece
    {
        public override int Value => 5;
        public override char NotationCharacter => 'R';

        public override List<MovementVector> MovementVectors { get; } = PieceMovementVectors.RookMovementVectors;
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
