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

        public GameProvider(IBotCreator botCreator, ISearch search)
        {
            this.botCreator = botCreator;
            this.search = search;
        }

        public Game StartNewGame(GameOptions gameOptions)
        {
            Game = new Game(gameOptions, botCreator);
            search.Initialize(Game);
            return Game;
        }
    }
}
