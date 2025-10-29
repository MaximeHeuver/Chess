namespace Chess.Engine.Bot.Models
{
    internal class PositionNode
    {
        public List<PositionNode> ChildPositions { get; set; }
        public string FenPosition { get; }
        public PositionEvaluation Evaluation { get; }

        public PositionNode(string fenPosition, PositionEvaluation evaluation)
        {
            FenPosition = fenPosition;
            Evaluation = evaluation;
        }
    }
}
