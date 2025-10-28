namespace Chess.Engine.GameModels.Pieces
{
    public static class PieceMovementVectors
    {
        public static List<MovementVector> GetPawnMovementVectors(Side side)
        {
            return side == Side.White
                ? WhitePawnMovementVectors
                : BlackPawnMovementVectors;
        }

        private static readonly List<MovementVector> WhitePawnMovementVectors = [
            new MovementVector(8, MovementCaptureOption.MoveOnly),
            new MovementVector(7, MovementCaptureOption.CaptureOnly),
            new MovementVector(9, MovementCaptureOption.CaptureOnly)
        ];

        private static readonly List<MovementVector> BlackPawnMovementVectors = [
            new MovementVector(-8, MovementCaptureOption.MoveOnly),
            new MovementVector(-7, MovementCaptureOption.CaptureOnly),
            new MovementVector(-9, MovementCaptureOption.CaptureOnly)
        ];

        public static readonly List<MovementVector> KnightMovementVectors =
        [
            new MovementVector(-17, MovementCaptureOption.Both),
            new MovementVector(-15, MovementCaptureOption.Both),
            new MovementVector(-10, MovementCaptureOption.Both),
            new MovementVector(-6, MovementCaptureOption.Both),
            new MovementVector(6, MovementCaptureOption.Both),
            new MovementVector(10, MovementCaptureOption.Both),
            new MovementVector(15, MovementCaptureOption.Both),
            new MovementVector(17, MovementCaptureOption.Both)
        ];

        public static readonly List<MovementVector> BishopMovementVectors =
        [
            new MovementVector(-9, MovementCaptureOption.Both),
            new MovementVector(-7, MovementCaptureOption.Both),
            new MovementVector(7, MovementCaptureOption.Both),
            new MovementVector(9, MovementCaptureOption.Both)
        ];

        public static readonly List<MovementVector> RookMovementVectors =
        [
            new MovementVector(-8, MovementCaptureOption.Both),
            new MovementVector(-1, MovementCaptureOption.Both),
            new MovementVector(8, MovementCaptureOption.Both),
            new MovementVector(1, MovementCaptureOption.Both)
        ];

        public static readonly List<MovementVector> QueenMovementVectors =
        [
            new MovementVector(-9, MovementCaptureOption.Both),
            new MovementVector(-8, MovementCaptureOption.Both),
            new MovementVector(-7, MovementCaptureOption.Both),
            new MovementVector(-1, MovementCaptureOption.Both),
            new MovementVector(1, MovementCaptureOption.Both),
            new MovementVector(7, MovementCaptureOption.Both),
            new MovementVector(8, MovementCaptureOption.Both),
            new MovementVector(9, MovementCaptureOption.Both)
        ];

        public static readonly List<MovementVector> KingMovementVectors =
        [
            new MovementVector(-9, MovementCaptureOption.Both),
            new MovementVector(-8, MovementCaptureOption.Both),
            new MovementVector(-7, MovementCaptureOption.Both),
            new MovementVector(-1, MovementCaptureOption.Both),
            new MovementVector(1, MovementCaptureOption.Both),
            new MovementVector(7, MovementCaptureOption.Both),
            new MovementVector(8, MovementCaptureOption.Both),
            new MovementVector(9, MovementCaptureOption.Both)
        ];
    }
}
