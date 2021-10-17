namespace Quoridor.Controller
{
    using Model;

    public class FlowController
    {
        private readonly MenuController menuController;
        private readonly GameController gameController;
        private readonly IGameStarter gameStarter;
        private readonly IIoWorker ioWorker;

        public FlowController(MenuController menuController, GameController gameController, IGameStarter gameStarter,
            IIoWorker ioWorker)
        {
            this.menuController = menuController;
            this.gameController = gameController;
            this.gameStarter = gameStarter;
            this.ioWorker = ioWorker;
        }

        public void StartNewGame()
        {
            if (menuController.TryGetGameOptions(out var gameOptions))
            {
                StartGame(gameOptions);
                return;
            }
            Quit();
        }

        private void StartGame(GameOptions gameOptions)
        {
            gameStarter.StartNewGame(gameOptions);
            var gameResult = gameController.StartGame();
            FinishGame(gameResult);
        }

        private void FinishGame(GameResult gameResult)
        {
            PrintGameResults(gameResult);
            StartNewGame();
        }

        private void PrintGameResults(GameResult gameResult)
        {
            ioWorker.WriteLine($"Winner: {gameResult.Winner.Name} player");
            ioWorker.WriteLine($"Number of moves: {gameResult.MoveCount}");
        }

        private void Quit()
        {
            ioWorker.WriteLine("See you next time");
        }
    }
}
