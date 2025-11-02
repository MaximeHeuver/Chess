using Chess.Engine.GameModels;

class Program
{

    static void Main()
    {
        Console.WriteLine("CPU cores: " + Environment.ProcessorCount);
        Console.WriteLine();

        Console.WriteLine("=== Sequential ===");
        RunSequential();

        Console.WriteLine();
        Console.WriteLine("=== Parallel ===");
        RunParallel();

        Console.WriteLine();
        Console.WriteLine("Done.");
    }

    static void DoWork(int taskId)
    {
        Console.WriteLine($"Task {taskId} started on thread {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(1000); // simulate 1 second of work
        Console.WriteLine($"Task {taskId} finished on thread {Thread.CurrentThread.ManagedThreadId}");
    }

    static void RunSequential()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 1; i <= 5; i++)
        {
            DoWork(i);
        }

        stopwatch.Stop();
        Console.WriteLine($"Sequential total time: {stopwatch.ElapsedMilliseconds} ms");
    }

    static void RunParallel()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        Parallel.For(1, 6, i => DoWork(i));

        stopwatch.Stop();
        Console.WriteLine($"Parallel total time: {stopwatch.ElapsedMilliseconds} ms");
    }

    // static void Main(string[] args)
    // {
    //     var game = new Game();
    //
    //     game.InitializeStandardGame();
    //
    //     while (true)
    //     {
    //         PrintCurrentPosition(game);
    //
    //         Console.WriteLine();
    //
    //         if (GameStateChecker.IsStaleMate(game))
    //         {
    //             Console.WriteLine($"Stalemate! {(game.Turn == Side.White ? "black" : "white")} wins!");
    //             break;
    //         }
    //
    //         if (GameStateChecker.IsKingCheckmate(game))
    //         {
    //             Console.WriteLine($"Checkmate! {(game.Turn == Side.White ? "black" : "white")} wins!");
    //             break;
    //         }
    //
    //         Console.WriteLine($"FEN notation: {FenConverter.GameToFenNotation(game)}");
    //
    //         Console.WriteLine();
    //
    //         Console.WriteLine("origin?");
    //
    //         var origin = Console.ReadLine();
    //
    //         Console.WriteLine("destination?");
    //
    //         var destination = Console.ReadLine();
    //
    //         Console.WriteLine();
    //
    //         try
    //         {
    //             var move = MoveHandler.GetMoveFromSquareNotation(game, origin, destination);
    //
    //             MoveHandler.ValidateAndExecuteMove(move, game);
    //         }
    //         catch (Exception e)
    //         {
    //             Console.Error.WriteLine(e.Message);
    //
    //             Console.WriteLine();
    //         }
    //
    //         Console.WriteLine($"King in check? {GameStateChecker.IsKingInCheck(game)}");
    //
    //         Console.WriteLine("-----------------------");
    //
    //         Console.WriteLine();
    //     }
    // }

    private static void PrintCurrentPosition(Game game)
    {
        var lines = Enumerable.Range(0, 8)
            .Select(rank =>
                string.Join(' ', Enumerable.Range(0, 8)
                    .Select(file => game.Board[(rank * 8) + file].PieceOccupation)))
            .ToList();

        for (var i = lines.Count - 1; i >= 0; i--)
        {
            Console.WriteLine(lines[i]);
        }
    }
}