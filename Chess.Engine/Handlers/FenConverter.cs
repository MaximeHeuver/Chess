using System.Text;

namespace Chess.Engine.Handlers
{
    public class FenConverter
    {
        public static string GameToFenNotation(Game game)
        {
            var stringBuilder = new StringBuilder();

            List<string> ranks = [];

            for (var i = 7; i >= 0; i--)
            {
                ranks.Add(RankToFenNotation(game.Board.Skip(i * 8).Take(8)));
            }

            stringBuilder.Append(string.Join('/', ranks));

            stringBuilder.Append(game.Turn == Side.White ? " w" : " b");

            AppendCastlingOptions(game.Board, stringBuilder);

            return stringBuilder.ToString();
        }

        private static void AppendCastlingOptions(List<Square> board, StringBuilder stringBuilder)
        {
            stringBuilder.Append(" ");

            if (MoveHandler.IsCastleOptionAvailable(Side.White, BoardSide.KingSide, board))
            {
                stringBuilder.Append("K");
            }

            if (MoveHandler.IsCastleOptionAvailable(Side.White, BoardSide.QueenSide, board))
            {
                stringBuilder.Append("Q");
            }

            if (MoveHandler.IsCastleOptionAvailable(Side.Black, BoardSide.KingSide, board))
            {
                stringBuilder.Append("k");
            }

            if (MoveHandler.IsCastleOptionAvailable(Side.Black, BoardSide.QueenSide, board))
            {
                stringBuilder.Append("q");
            }

            if (stringBuilder.Length == 0)
            {
                stringBuilder.Append("-");
            }
        }

        private static string RankToFenNotation(IEnumerable<Square> rank)
        {
            var stringBuilder = new StringBuilder();

            var currentNumberOfEmptySquares = 0;

            foreach (var square in rank)
            {
                if (square.Piece == null)
                {
                    currentNumberOfEmptySquares++;
                    continue;
                }

                var pieceNotationCharacter = square.Piece.Side == Side.White
                    ? square.Piece.NotationCharacter.ToString().ToUpper()
                    : square.Piece.NotationCharacter.ToString().ToLower();

                if (currentNumberOfEmptySquares != 0)
                {
                    stringBuilder.Append(currentNumberOfEmptySquares);
                    currentNumberOfEmptySquares = 0;
                }

                stringBuilder.Append(pieceNotationCharacter);
            }

            if (currentNumberOfEmptySquares != 0)
            {
                stringBuilder.Append(currentNumberOfEmptySquares);
            }

            return stringBuilder.ToString();
        }
    }
}
