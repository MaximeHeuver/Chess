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

        public Knight(Side side) : base(side)
        {
        }
        public Knight() : base(Side.White)
        {
        }
    }
}
