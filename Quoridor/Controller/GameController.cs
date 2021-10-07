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
            bluePlayerMover = new PlayerMover(gameProvider, gameProvider.Game.BluePlayer, moveParser);
            redPlayerMover = new PlayerMover(gameProvider, gameProvider.Game.RedPlayer, moveParser);
            while (!gameProvider.Game.IsFinished)
            {
                DrawField();
                MakeMove(bluePlayerMover);
                DrawField();
                MakeMove(redPlayerMover);
            }
        }

        private void DrawField()
        {
            fieldView.Draw(gameProvider.Game.Field, gameProvider.Game.BluePlayer, gameProvider.Game.RedPlayer);
        }

        private void MakeMove(PlayerMover playerMover)
        {
            var move = playerMover.WaitForMove();
            if (!move.IsValid())
            {
                PrintMessage();
                return;
            }
            move.Execute();
            moveQueue.Add(move);
        }

        private void PrintMessage()
        {
            Console.WriteLine("Invalid move");
        }
    }
}
