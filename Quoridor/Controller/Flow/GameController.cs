namespace Quoridor.Controller.Flow
{
    using Game;
    using Io;
    using Model.Moves;
    using Moves;
    using View;

    public class GameController
    {
        private readonly IGameProvider gameProvider;
        private readonly IMoveConverter moveConverter;
        private readonly IIoWorker ioWorker;
        private readonly FieldView fieldView;

        private PlayerMover bluePlayerMover;
        private PlayerMover redPlayerMover;
        private int moveCount;
        public static IMove lastMove;

        public GameController(IGameProvider gameProvider, IMoveConverter moveConverter, IIoWorker ioWorker)
        {
            this.gameProvider = gameProvider;
            this.moveConverter = moveConverter;
            this.ioWorker = ioWorker;
            fieldView = new FieldView();
        }

        public void StartGame()
        {
            PrepareComponents();
            DrawField();
            ProcessGame();
        }

        private void PrepareComponents()
        {
            bluePlayerMover = new PlayerMover(gameProvider.Game.Field, gameProvider.Game.BluePlayer, moveConverter, ioWorker);
            redPlayerMover = new PlayerMover(gameProvider.Game.Field, gameProvider.Game.RedPlayer, moveConverter, ioWorker);
            moveCount = 0;
        }

        private void ProcessGame()
        {
            while (!gameProvider.Game.HasFinished())
            {
                var mover = GetMover();
                MakeMove(mover);
                DrawField();
                moveCount++;
            }
        }

        private PlayerMover GetMover()
        {
            return moveCount % 2 == 0 ? bluePlayerMover : redPlayerMover;
        }

        private void MakeMove(PlayerMover playerMover)
        {
            lastMove = playerMover.WaitForMove();
            lastMove.Execute();
        }

        private void DrawField()
        {
            fieldView.Draw(gameProvider.Game.Field, gameProvider.Game.BluePlayer, gameProvider.Game.RedPlayer);
        }
    }
}
