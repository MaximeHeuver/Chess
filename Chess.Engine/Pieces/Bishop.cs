namespace Chess.Engine.Pieces
{
    public class Bishop : Piece
    {
        public override int Value => 3;
        public override char NotationCharacter => 'B';
        public override int[] MovementVectors => [-9, -7, 7, 9];
        public override bool CanPieceSlide => true;
        public override int[] CaptureVectors => MovementVectors;

        public Bishop(Side side) : base(side)
        {
        }

        public Bishop() : base(Side.White)
        {
        }
    }
}
