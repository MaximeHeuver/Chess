namespace Chess.Engine.GameModels
{
    public class MovementVector
    {
        public int Vector { get; }
        public bool CanMovementCapture { get; }

        public MovementVector(int vector, bool canMovementCapture)
        {
            Vector = vector;
            CanMovementCapture = canMovementCapture;
        }
    }
}
