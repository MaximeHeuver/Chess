using Chess.Engine.GameModels;

namespace Chess.Engine.Bot.Models
{
    public class PositionNode
    {
        public List<PositionNode> ChildPositions { get; set; } = [];
        public string FenPosition { get; }
        public int Evaluation { get; set; }
        public Move? Move { get; set; }

        public bool IsStoppedDueToPruning { get; set; } = false;

        public PositionNode(string fenPosition, Move? move)
        {
            FenPosition = fenPosition;
            Move = move;
        }
    }
}
