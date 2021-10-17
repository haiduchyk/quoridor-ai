namespace Quoridor.Controller
{
    using Model;

    public class GameContainer
    {
        public FlowController FlowController { get; }

        public GameContainer()
        {
            var wallProvider = new WallProvider();
            var moveProvider = new MoveProvider();
            var botCreator = new BotCreator(moveProvider, wallProvider);
            var moveParser = new MoveParser(moveProvider, wallProvider);
            var gameProvider = new GameProvider(botCreator);
            var ioWorker = new ConsoleWorker();
            var menuController = new MenuController(ioWorker);
            var gameController = new GameController(gameProvider, moveParser, ioWorker);
            FlowController = new FlowController(menuController, gameController, gameProvider, ioWorker);
        }
    }
}
