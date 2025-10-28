namespace Chess.Engine.GameModels.Pieces
{
    public class Queen : Piece
    {
        public override int Value => 9;
        public override char NotationCharacter => 'Q';

        public override List<MovementVector> MovementVectors { get; } = PieceMovementVectors.QueenMovementVectors;

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
    }
}
