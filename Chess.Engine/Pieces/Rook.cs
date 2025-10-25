using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class Rook : Piece
    {
        public override int Value => 5;
        public override char NotationCharacter => 'R';

        public override List<MovementVector> MovementVectors { get; } =
        [
            new MovementVector(-8, true),
            new MovementVector(-1, true),
            new MovementVector(8, true),
            new MovementVector(1, true)
        ];
        public override bool CanPieceSlide => true;

        public Rook(Side side) : base(side)
        {
        }

        public Rook() : base(Side.White)
        {
        }
    }
}
