namespace Chess.Engine.Pieces
{
    public class Pawn : Piece
    {
        public override int Value => 1;
        public override char NotationCharacter => 'P';
        public override int[] MovementVectors => this.Side == Side.White ? [8] : [-8];
        public override bool CanPieceSlide => false;
        public override int[] CaptureVectors => this.Side == Side.White ? [7, 9] : [-7, -9];

        public Pawn(Side side) : base(side)
        {
        }
        public Pawn() : base(Side.White)
        {
        }
    }
}
