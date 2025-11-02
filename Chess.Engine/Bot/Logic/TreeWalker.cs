using Chess.Engine.Bot.Models;

namespace Chess.Engine.Bot.Logic
{
    public static class TreeWalker
    {
        public static int GetNumberOfPrunedNodes(PositionNode? node)
        {
            return node == null ? 0 : GetNodeTotal(node);
        }

        private static int GetNodeTotal(PositionNode node)
        {
            if (node.ChildPositions.Count == 0)
            {
                return 0;
            }
            if (node.IsStoppedDueToPruning)
            {
                return 1;
            }

            return node.ChildPositions.Sum(GetNodeTotal);
        }
    }
}
