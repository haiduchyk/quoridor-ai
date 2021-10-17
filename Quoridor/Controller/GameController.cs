namespace Quoridor.Controller
{
    using System;
    using View;

    public class GameController
    {
        private readonly IGameProvider gameProvider;
        private readonly IMoveParser moveParser;
        private readonly IIoWorker ioWorker;
        private readonly MoveQueue moveQueue;
        private readonly FieldView fieldView;

        private PlayerMover bluePlayerMover;
        private PlayerMover redPlayerMover;

        public GameController(IGameProvider gameProvider, IMoveParser moveParser, IIoWorker ioWorker)
        {
            this.gameProvider = gameProvider;
            this.moveParser = moveParser;
            this.ioWorker = ioWorker;
            moveQueue = new MoveQueue();
            fieldView = new FieldView();
        }

        public void StartGame()
        {
            moveQueue.Clear();
            bluePlayerMover = new PlayerMover(gameProvider, gameProvider.Game.BluePlayer, gameProvider.Game.RedPlayer,
                moveParser, ioWorker);
            redPlayerMover = new PlayerMover(gameProvider, gameProvider.Game.RedPlayer, gameProvider.Game.BluePlayer,
                moveParser, ioWorker);
            ProcessGame();
        }

        private void ProcessGame()
        {
            var moveCount = 0;
            while (!gameProvider.Game.HasFinished())
            {
                var mover = GetMover(moveCount);
                Update(mover);
                moveCount++;
            }
            EndGame(moveCount);
        }

        private PlayerMover GetMover(int moveCount)
        {
            return moveCount % 2 == 0 ? bluePlayerMover : redPlayerMover;
        }

        private void Update(PlayerMover mover)
        {
            DrawField();
            MakeMove(mover);
        }

        private void DrawField()
        {
            fieldView.Draw(gameProvider.Game.Field, gameProvider.Game.BluePlayer, gameProvider.Game.RedPlayer);
        }

        private void MakeMove(PlayerMover playerMover)
        {
            var move = playerMover.WaitForMove();
            while (!move.IsValid())
            {
                PrintMessage();
                move = playerMover.WaitForMove();
            }
            move.Execute();
            moveQueue.Add(move);
        }

        private void PrintMessage()
        {
            // Console.WriteLine("Invalid move");
        }

        private void EndGame(int moveCount)
        {
            DrawField();
            var winner = (moveCount - 1) % 2 == 0 ? "Blue" : "Red";
            Console.WriteLine($"Winner: {winner} player");
            Console.WriteLine($"Number of moves: {moveCount}");
        }
    }
}
