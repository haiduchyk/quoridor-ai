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

        public GameProvider(IBotCreator botCreator)
        {
            this.botCreator = botCreator;
        }

        public Game StartNewGame(GameOptions gameOptions)
        {
            Game = new Game(gameOptions, botCreator);
            return Game;
        }
    }
}
