using Chess.Engine.GameModels;
using System.Text.RegularExpressions;

namespace Chess.Engine.Logic
{
    public class ChessNotationConverter
    {
        public static Square GetSquareFromAbbreviation(List<Square> board, string abbreviation)
        {
            if (!SquareNotationFormat.IsMatch(abbreviation))
            {
                throw new ArgumentException($"{abbreviation} is not a valid square name");
            }

            var index = (abbreviation[0] - 97) + (abbreviation[1] - 49) * 8;

            return board[index];
        }

        private static readonly Regex SquareNotationFormat = new Regex("^[a-h][1-8]$");

        public static string GetAbbreviationFromIndex(int boardIndex)
        {
            return $"{(char)(boardIndex % 8 + 97)}{(char)(boardIndex / 8 + 49)}";
        }
    }
}
