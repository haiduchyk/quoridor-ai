namespace Quoridor.Controller
{
    using Model;
    using Model.Moves;
    using Model.Players;
    using View;

    public class GameController
    {
        private const string ExampleMove = "8g 8e";
        private const string ExampleWall = "11 h or 11 v";

        private readonly IGameProvider gameProvider;
        private readonly IMoveParser moveParser;
        private readonly IIoWorker ioWorker;
        private readonly MoveQueue moveQueue;
        private readonly FieldView fieldView;

        private PlayerMover bluePlayerMover;
        private PlayerMover redPlayerMover;
        private int moveCount;

        public GameController(IGameProvider gameProvider, IMoveParser moveParser, IIoWorker ioWorker)
        {
            this.gameProvider = gameProvider;
            this.moveParser = moveParser;
            this.ioWorker = ioWorker;
            moveQueue = new MoveQueue();
            fieldView = new FieldView();
        }

        public GameResult StartGame()
        {
            PrepareComponents();
            DrawField();
            PrintHelpMessage();
            ProcessGame();
            return GatherGameResults();
        }

        private void PrepareComponents()
        {
            moveQueue.Clear();
            bluePlayerMover = new PlayerMover(gameProvider.Game, gameProvider.Game.BluePlayer, moveParser, ioWorker);
            redPlayerMover = new PlayerMover(gameProvider.Game, gameProvider.Game.RedPlayer, moveParser, ioWorker);
            moveCount = 0;
        }

        private void ProcessGame()
        {
            while (!gameProvider.Game.HasFinished())
            {
                var mover = GetMover();
                MakeMove(mover);
                DrawField();
                PrintPlayerInfo();
                moveCount++;
            }
        }

        private PlayerMover GetMover()
        {
            return moveCount % 2 == 0 ? bluePlayerMover : redPlayerMover;
        }

        private void MakeMove(PlayerMover playerMover)
        {
            while (true)
            {
                var move = playerMover.WaitForMove();
                if (move.IsValid())
                {
                    Execute(move);
                    return;
                }
                PrintInvalidMessage();
                PrintHelpMessage();
            }
        }

        private void Execute(IMove move)
        {
            move.Execute();
            moveQueue.Add(move);
        }

        private void DrawField()
        {
            fieldView.Draw(gameProvider.Game.Field, gameProvider.Game.BluePlayer, gameProvider.Game.RedPlayer);
        }

        private void PrintPlayerInfo()
        {
            PrintWallInfo(gameProvider.Game.BluePlayer);
            PrintWallInfo(gameProvider.Game.RedPlayer);
        }

        private void PrintWallInfo(Player player)
        {
            ioWorker.WriteLine($"{player.Name} walls: {player.AmountOfWalls}");
        }

        private void PrintInvalidMessage()
        {
            ioWorker.WriteLine("Invalid move");
        }

        private void PrintHelpMessage()
        {
            ioWorker.WriteLine($"Move example: {ExampleMove}");
            ioWorker.WriteLine($"Walls example: {ExampleWall}");
        }

        private GameResult GatherGameResults()
        {
            var winner = (moveCount - 1) % 2 == 0 ? gameProvider.Game.BluePlayer : gameProvider.Game.RedPlayer;
            return new GameResult(winner, moveCount);
        }
    }
}
