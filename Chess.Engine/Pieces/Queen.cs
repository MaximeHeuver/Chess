using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class Queen : Piece
    {
        public override int Value => 9;
        public override char NotationCharacter => 'Q';

        public override List<MovementVector> MovementVectors { get; } =
        [
            new MovementVector(-9, true),
            new MovementVector(-8, true),
            new MovementVector(-7, true),
            new MovementVector(-1, true),
            new MovementVector(1, true),
            new MovementVector(7, true),
            new MovementVector(8, true),
            new MovementVector(9, true)
        ];

        public override bool CanPieceSlide => true;

        public Queen(Side side) : base(side)
        {
        }

        public Queen() : base(Side.White)
        {
        }
    }
}
