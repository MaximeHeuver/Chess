using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class Knight : Piece
    {
        public override int Value => 3;
        public override char NotationCharacter => 'N';

        public override List<MovementVector> MovementVectors { get; } =
        [
            new MovementVector(-17, true),
            new MovementVector(-15, true),
            new MovementVector(-10, true),
            new MovementVector(-6, true),
            new MovementVector(6, true),
            new MovementVector(10, true),
            new MovementVector(15, true),
            new MovementVector(17, true)
        ];

        public override bool CanPieceSlide => false;

        public override Piece DeepCopy()
        {
            return new Knight(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private Knight(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }
        public Knight(Side side) : base(side)
        {
        }
        public Knight() : base(Side.White)
        {
        }
    }
}
