using Chess.Engine.GameModels;

namespace Chess.Engine.Pieces
{
    public class Pawn : Piece
    {
        public override int Value => 1;
        public override char NotationCharacter => 'P';

        public override List<MovementVector> MovementVectors => this.Side == Side.White
            ? this.HasPieceMoved
                ? [
                    new MovementVector(8, false),
                    new MovementVector(7, true),
                    new MovementVector(9, true)
                ]
                : [
                    new MovementVector(8, false),
                    new MovementVector(7, true),
                    new MovementVector(9, true),
                    new MovementVector(16, false)]
            : this.HasPieceMoved
                ? [
                    new MovementVector(8, false),
                    new MovementVector(7, true),
                    new MovementVector(9, true)
                ]
                : [
                    new MovementVector(-8, false),
                    new MovementVector(-7, true),
                    new MovementVector(-9, true),
                    new MovementVector(-16, false)
                ];

        public override bool CanPieceSlide => false;

        public override Piece DeepCopy()
        {
            return new Pawn(Side == Side.White ? Side.White : Side.Black, HasPieceMoved);
        }
        private Pawn(Side side, bool hasPieceMoved) : base(side, hasPieceMoved)
        {
        }
        public Pawn(Side side) : base(side)
        {
        }

        public Pawn() : base(Side.White)
        {
        }
    }
}
