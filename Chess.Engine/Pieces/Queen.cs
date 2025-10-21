namespace Chess.Engine.Pieces
{
    public class Queen : Piece
    {
        public override int Value => 9;
        public override char NotationCharacter => 'Q';
        public override int[] MovementVectors => [-9, -8, -7, -1, 1, 7, 8, 9];
        public override bool CanPieceSlide => true;
        public override int[] CaptureVectors => MovementVectors;

        public Queen(Side side) : base(side)
        {
        }

        public Queen() : base(Side.White)
        {
        }
    }
}
