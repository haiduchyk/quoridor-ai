namespace Quoridor.Controller
{
    using System;
    using View;

    public class GameController
    {
        private readonly IGameProvider gameProvider;
        private readonly IMoveParser moveParser;
        private readonly MoveQueue moveQueue;
        private readonly FieldView fieldView;

        private PlayerMover bluePlayerMover;
        private PlayerMover redPlayerMover;

        public GameController(IGameProvider gameProvider, IMoveParser moveParser)
        {
            this.gameProvider = gameProvider;
            this.moveParser = moveParser;
            moveQueue = new MoveQueue();
            fieldView = new FieldView();
        }

        public void ProcessGame()
        {
            moveQueue.Clear();
            bluePlayerMover = new PlayerMover(gameProvider, gameProvider.Game.BluePlayer, gameProvider.Game.RedPlayer, moveParser);
            redPlayerMover = new PlayerMover(gameProvider, gameProvider.Game.RedPlayer,gameProvider.Game.BluePlayer, moveParser);
            var moveCount = 0;
            while (!gameProvider.Game.IsFinished)
            {
                Console.ReadLine();
                var mover = moveCount % 2 == 0 ? bluePlayerMover : redPlayerMover;
                DrawField();
                MakeMove(mover);
                moveCount++;
            }
            EndGame(moveCount);
        }

        private void EndGame(int moveCount)
        {
            DrawField();
            var winner = (moveCount - 1) % 2 == 0 ? "Blue" : "Red";
            Console.WriteLine($"Winner: {winner} player");
            Console.WriteLine($"Number of moves: {moveCount}");
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
    }
}
