namespace Chess.Engine.Bot.Models
{
    public class PositionEvaluation
    {
        public int WhiteRating { get; set; }
        public int BlackRating { get; set; }
        public int SummedRatings => WhiteRating - BlackRating;

        public PositionEvaluation(int whiteRating, int blackRating)
        {
            WhiteRating = whiteRating;
            BlackRating = blackRating;
        }
    }
}
