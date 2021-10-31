namespace Quoridor.Controller.Game
{
    using Model;
    using Model.Strategies;

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

        private readonly IPlayerCreator playerCreator;
        private readonly IWallProvider wallProvider;

        public GameProvider(IPlayerCreator playerCreator, IWallProvider wallProvider)
        {
            this.playerCreator = playerCreator;
            this.wallProvider = wallProvider;
        }

        public Game StartNewGame(GameOptions gameOptions)
        {
            var field = new Field();
            // TODO index
            // field.PossibleWalls.AddRange(wallProvider.GetAllMoves());
            var bluePlayer = playerCreator.CreateFirstPlayer(gameOptions);
            var redPlayer = playerCreator.CreateSecondPlayer(gameOptions);
            bluePlayer.SetEnemy(redPlayer);
            redPlayer.SetEnemy(bluePlayer);
            Game = new Game(field, bluePlayer, redPlayer);
            return Game;
        }
    }
}
