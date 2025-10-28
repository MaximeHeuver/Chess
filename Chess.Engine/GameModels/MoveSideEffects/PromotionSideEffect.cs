using Chess.Engine.GameModels.Pieces;

namespace Chess.Engine.GameModels.MoveSideEffects
{
    public class PromotionSideEffect : MoveSideEffect
    {
        public Square SquareWithPieceToPromote { get; set; }

        public PromotionSideEffect(Square squareWithPieceToPromote)
        {
            SquareWithPieceToPromote = squareWithPieceToPromote;
        }

        public override void Execute()
        {
            SquareWithPieceToPromote.Piece = new Queen(SquareWithPieceToPromote.Piece!.Side);
        }
    }
}
