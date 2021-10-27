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
        private readonly IWallProvider wallProvider;

        public GameProvider(IBotCreator botCreator, IWallProvider wallProvider)
        {
            this.botCreator = botCreator;
            this.wallProvider = wallProvider;
        }

        public Game StartNewGame(GameOptions gameOptions)
        {
            Game = new Game(gameOptions, botCreator, wallProvider);
            return Game;
        }
    }
}
