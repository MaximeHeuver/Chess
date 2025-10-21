namespace Chess.Engine.Pieces
{
    public class Knight : Piece
    {
        public override int Value => 3;
        public override char NotationCharacter => 'N';
        public override int[] MovementVectors => [-17, -15, -10, -6, 6, 10, 15, 17];
        public override bool CanPieceSlide => false;
        public override int[] CaptureVectors => MovementVectors;

        public Knight(Side side) : base(side)
        {
        }
        public Knight() : base(Side.White)
        {
        }
    }
}
