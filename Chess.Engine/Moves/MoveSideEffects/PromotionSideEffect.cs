using Chess.Engine.Pieces;

namespace Chess.Engine.Moves.MoveSideEffects
{
    public class PromotionSideEffect : MoveSideEffect
    {
        public Piece PieceToPromote { get; set; }

        public PromotionSideEffect(Piece pieceToPromote)
        {
            PieceToPromote = pieceToPromote;
        }

        public override void Execute()
        {
            PieceToPromote = new Queen(PieceToPromote.Side);
        }
    }
}
