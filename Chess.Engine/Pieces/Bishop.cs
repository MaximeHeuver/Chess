using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class Bishop : Piece
    {
        public override int Value => 3;
        public override char NotationCharacter => 'B';

        public override List<MovementVector> MovementVectors { get; } =
        [
            new MovementVector(-9, true),
            new MovementVector(-7, true),
            new MovementVector(7, true),
            new MovementVector(9, true)
        ];

        public override bool CanPieceSlide => true;
        public override Piece DeepCopy()
        {
            return new Bishop(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }

        private Bishop(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }

        public Bishop(Side side) : base(side)
        {
        }

        public Bishop() : base(Side.White)
        {
        }
    }
}
