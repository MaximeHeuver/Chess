namespace Chess.Engine.Bot.Models
{
    public class PositionEvaluation
    {
        public decimal WhiteRating { get; set; }
        public decimal BlackRating { get; set; }

        public (decimal whiteFraction, decimal blackFraction) WhiteToBlackRatio => (
            WhiteRating / WhiteRating + BlackRating,
            BlackRating / WhiteRating + BlackRating
        );

        public PositionEvaluation(decimal whiteRating, decimal blackRating)
        {
            WhiteRating = whiteRating;
            BlackRating = blackRating;
        }
    }
}
