namespace Chess.Engine.Pieces
{
    public class Rook : Piece
    {
        public override int Value => 5;
        public override char NotationCharacter => 'R';
        public override int[] MovementVectors => [-8, -1, 1, 8];
        public override bool CanPieceSlide => true;
        public override int[] CaptureVectors => MovementVectors;

        public Rook(Side side) : base(side)
        {
        }

        public Rook() : base(Side.White)
        {
        }
    }
}
