namespace Chess.Engine.GameModels
{
    public class MovementVector
    {
        public int Vector { get; }
        public MovementCaptureOption MovementCaptureOption { get; }

        public MovementVector(int vector, MovementCaptureOption movementCaptureOption)
        {
            Vector = vector;
            MovementCaptureOption = movementCaptureOption;
        }
    }
}
