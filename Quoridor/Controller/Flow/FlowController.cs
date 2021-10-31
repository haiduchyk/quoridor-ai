namespace Quoridor.Controller.Flow
{
    using Game;

    public class FlowController
    {
        private readonly MenuController menuController;
        private readonly GameController gameController;
        private readonly IGameStarter gameStarter;

        public FlowController(MenuController menuController, GameController gameController, IGameStarter gameStarter)
        {
            this.menuController = menuController;
            this.gameController = gameController;
            this.gameStarter = gameStarter;
        }

        public void StartNewGame()
        {
            var gameOptions = menuController.GetGameOptions();
            StartGame(gameOptions);
        }

        private void StartGame(GameOptions gameOptions)
        {
            gameStarter.StartNewGame(gameOptions);
            gameController.StartGame();
        }
    }
}
