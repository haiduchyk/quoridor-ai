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
            var pathRetriever = new PathWithWallsRetriever();
            var search = new AStarSearchAlgorithm(moveProvider, pathRetriever);
            var botCreator = new BotCreator(moveProvider, wallProvider, search);
            var moveParser = new MoveParser(moveProvider, wallProvider, search);
            var gameProvider = new GameProvider(botCreator, search);
            var ioWorker = new ConsoleWorker();
            var menuController = new MenuController(ioWorker);
            var gameController = new GameController(gameProvider, moveParser, ioWorker);
            FlowController = new FlowController(menuController, gameController, gameProvider, ioWorker);
        }
    }
}
