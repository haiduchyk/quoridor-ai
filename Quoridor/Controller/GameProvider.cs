namespace Quoridor.Controller
{
    using Model;

    public interface IGameProvider
    {
        Game Game { get; }
    }

    public interface IGameStarter
    {
        Game StartNewGame(GameOptions gameOptions);
    }

    public class GameProvider : IGameProvider, IGameStarter
    {
        public Game Game { get; private set; }

        private readonly IBotCreator botCreator;
        private readonly ISearch search;
        private readonly IWallProvider wallProvider;

        public GameProvider(IBotCreator botCreator, IWallProvider wallProvider, ISearch search)
        {
            this.botCreator = botCreator;
            this.search = search;
            this.wallProvider = wallProvider;
        }

        public Game StartNewGame(GameOptions gameOptions)
        {
            Game = new Game(gameOptions, botCreator, wallProvider);
            search.Initialize(Game);
            return Game;
        }
    }
}
