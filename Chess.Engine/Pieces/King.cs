using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class King : Piece
    {
        public override int Value => 999;
        public override char NotationCharacter => 'K';
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

        public King(Side side) : base(side)
        {
        }
    }
}
