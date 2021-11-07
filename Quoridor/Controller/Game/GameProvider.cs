namespace Quoridor.Controller.Game
{
    using System.Linq;
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
        private readonly ISearch search;

        public GameProvider(IPlayerCreator playerCreator, IWallProvider wallProvider, ISearch search)
        {
            this.playerCreator = playerCreator;
            this.wallProvider = wallProvider;
            this.search = search;
        }

        public Game StartNewGame(GameOptions gameOptions)
        {
            var field = new Field(search);
            field.PossibleWalls.AddRange(wallProvider.GetAllMoves());
            
            var bluePlayer = playerCreator.CreateFirstPlayer(gameOptions);
            var redPlayer = playerCreator.CreateSecondPlayer(gameOptions);
            bluePlayer.SetEnemy(redPlayer);
            redPlayer.SetEnemy(bluePlayer);
            Game = new Game(field, bluePlayer, redPlayer);
            return Game;
        }
    }
}
