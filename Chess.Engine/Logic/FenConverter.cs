using Chess.Engine.GameModels;
using Chess.Engine.GameModels.Pieces;
using System.Text;
using System.Text.RegularExpressions;

namespace Chess.Engine.Logic
{
    public class FenConverter
    {
        private static Regex castleRegex = new Regex("^[K?Q?k?q?]$");

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

            stringBuilder.Append(WasLastMoveEnPassant(game) ? $" {ChessNotationConverter.GetAbbreviationFromIndex(game.LastPlayedMove.Destination.BoardIndex)}" : " -");

            return stringBuilder.ToString();
        }

        private static bool WasLastMoveEnPassant(Game game)
        {
            return game.LastPlayedMove is { Destination.Piece: Pawn } &&
                   Math.Abs(game.LastPlayedMove.Origin.BoardIndex - game.LastPlayedMove.Destination.BoardIndex) == 16;
        }

        private static void AppendCastlingOptions(List<Square> board, StringBuilder stringBuilder)
        {
            List<string> stringsToAppend = [];

            if (GameStateChecker.IsCastleOptionAvailable(Side.White, BoardSide.KingSide, board))
            {
                stringsToAppend.Add("K");
            }

            if (GameStateChecker.IsCastleOptionAvailable(Side.White, BoardSide.QueenSide, board))
            {
                stringsToAppend.Add("Q");
            }

            if (GameStateChecker.IsCastleOptionAvailable(Side.Black, BoardSide.KingSide, board))
            {
                stringsToAppend.Add("k");
            }

            if (GameStateChecker.IsCastleOptionAvailable(Side.Black, BoardSide.QueenSide, board))
            {
                stringsToAppend.Add("q");
            }

            if (stringsToAppend.Count == 0)
            {
                stringBuilder.Append(" -");
            }
            else
            {
                stringBuilder.Append(" ");
                stringsToAppend.ForEach(x => stringBuilder.Append(x));
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

        public static Game FenNotationToGame(string nodeFenPosition)
        {
            var sections = nodeFenPosition.Split(' ').ToList();

            var rows = sections[0].Split('/');
            List<Square> board = [];

            for (int i = 0; i < rows.Length; i++)
            {
                board.AddRange(RowToSquares(rows[i], i));
            }

            var side = GetSide(sections[1]);

            ParseCastleOptions(sections[2], board);

            // var lastMove = GetLastMove(sections[3]);

            //todo last move

            return new Game(board, side, null);
        }

        private static void ParseCastleOptions(string section, List<Square> board)
        {
            if (section == "-")
            {
                return;
            }

            if (!castleRegex.IsMatch(section))
            {
                throw new ArgumentException($"unable to convert fen position. castle options {section} is not known");
            }

            if (section.Contains("K"))
            {
                SquareFinder.GetKingStartingSquare(Side.White, board).Piece!.HasPieceMoved = false;
                SquareFinder.GetRookStartingSquare(Side.White, BoardSide.KingSide, board).Piece!.HasPieceMoved = false;
            }

            if (section.Contains("Q"))
            {
                SquareFinder.GetKingStartingSquare(Side.White, board).Piece!.HasPieceMoved = false;
                SquareFinder.GetRookStartingSquare(Side.White, BoardSide.QueenSide, board).Piece!.HasPieceMoved = false;
            }

            if (section.Contains("k"))
            {
                SquareFinder.GetKingStartingSquare(Side.Black, board).Piece!.HasPieceMoved = false;
                SquareFinder.GetRookStartingSquare(Side.Black, BoardSide.KingSide, board).Piece!.HasPieceMoved = false;
            }

            if (section.Contains("q"))
            {
                SquareFinder.GetKingStartingSquare(Side.Black, board).Piece!.HasPieceMoved = false;
                SquareFinder.GetRookStartingSquare(Side.Black, BoardSide.QueenSide, board).Piece!.HasPieceMoved = false;
            }
        }

        private static Side GetSide(string sideSection)
        {
            return sideSection switch
            {
                "w" => Side.White,
                "b" => Side.Black,
                _ => throw new ArgumentException($"unable to convert fen position. side {sideSection} is not known")
            };
        }

        private static List<Square> RowToSquares(string row, int rank)
        {
            List<Square> rowSquares = [];

            var currentIndexInRow = 0;

            foreach (var character in row)
            {
                if (int.TryParse(character.ToString(), out var amountOFEmptySquares))
                {
                    for (int i = 0; i < amountOFEmptySquares; i++)
                    {
                        rowSquares.Add(new Square(rank * 8 + currentIndexInRow));
                        currentIndexInRow++;
                    }

                    continue;
                }

                rowSquares.Add(new Square(rank * 8 + currentIndexInRow, GetPieceFromLetter(character)));
                currentIndexInRow++;
            }

            return rowSquares;
        }

        private static Piece GetPieceFromLetter(char character)
        {
            return character switch
            {
                'k' => new King(Side.Black),
                'K' => new King(Side.White),
                'q' => new Queen(Side.Black),
                'Q' => new Queen(Side.White),
                'r' => new Rook(Side.Black),
                'R' => new Rook(Side.White),
                'b' => new Bishop(Side.Black),
                'B' => new Bishop(Side.White),
                'n' => new Knight(Side.Black),
                'N' => new Knight(Side.White),
                'p' => new Pawn(Side.Black),
                'P' => new Pawn(Side.White),
                _ => throw new ArgumentException($"unable to convert fen position. piece {character} is not known")
            };
        }
    }
}
