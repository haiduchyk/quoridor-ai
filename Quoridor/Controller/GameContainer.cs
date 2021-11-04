namespace Quoridor.Controller
{
    using Flow;
    using Game;
    using Io;
    using Model;
    using Moves;

    public class GameContainer
    {
        public FlowController FlowController { get; }

        public GameContainer()
        {
            var moveProvider = new MoveProvider();
            var wallProvider = new WallProvider(moveProvider);
            var pathRetriever = new PathRetriever();
            var search = new AStarSearchAlgorithm(moveProvider, pathRetriever);
            var botCreator = new PlayerCreator(moveProvider, wallProvider, search);
            var positionConverter = new PositionConverter(wallProvider);
            var moveConverter = new MoveConverter(positionConverter, moveProvider, wallProvider, search);
            var gameProvider = new GameProvider(botCreator, wallProvider, search);
            var ioWorker = new ConsoleWorker();
            var menuController = new MenuController(ioWorker);
            var gameController = new GameController(gameProvider, moveConverter, ioWorker);
            FlowController = new FlowController(menuController, gameController, gameProvider);
        }
    }
}
