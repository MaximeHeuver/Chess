namespace Chess.Engine.Pieces
{
    public class King : Piece
    {
        public override int Value => 999;
        public override char NotationCharacter => 'K';
        public override int[] MovementVectors => [-9, -8, -7, -1, 1, 7, 8, 9];
        public override bool CanPieceSlide => true;
        public override int[] CaptureVectors => MovementVectors;

        public King(Side side) : base(side)
        {
        }
    }
}
