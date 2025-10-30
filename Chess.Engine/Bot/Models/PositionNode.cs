using Chess.Engine.GameModels;

namespace Chess.Engine.Bot.Models
{
    internal class PositionNode
    {
        public List<PositionNode> ChildPositions { get; set; } = [];
        public string FenPosition { get; }
        public decimal Evaluation { get; }
        public bool IsMax { get; set; }
        public Move? Move { get; set; }

        public decimal MinMaxedEvaluation => IsMax
            ? ChildPositions.Max(x => x.Evaluation)
            : ChildPositions.Min(x => x.Evaluation);

        public PositionNode(string fenPosition, decimal evaluation, bool isMax, Move? move)
        {
            FenPosition = fenPosition;
            Evaluation = evaluation;
            IsMax = isMax;
            Move = move;
        }
    }
}
