namespace Quoridor.Controller
{
    using Model;

    public class GameContainer
    {
        public IGameProvider GameProvider { get; }

        public MenuController MenuController { get; }

        public GameController GameController { get; }

        public GameContainer()
        {
            var wallProvider = new WallProvider();
            var moveProvider = new MoveProvider();
            var botCreator = new BotCreator(moveProvider);
            var moveParser = new MoveParser(moveProvider, wallProvider);
            var gameProvider = new GameProvider(botCreator);
            GameProvider = gameProvider;
            GameController = new GameController(gameProvider, moveParser);
            MenuController = new MenuController(gameProvider, GameController);
        }
    }
}
