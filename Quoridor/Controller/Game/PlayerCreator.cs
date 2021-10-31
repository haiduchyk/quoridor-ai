namespace Quoridor.Controller.Game
{
    using Model;
    using Model.Players;
    using Model.Strategies;

    public interface IPlayerCreator
    {
        Player CreateFirstPlayer(GameOptions gameOptions);

        Player CreateSecondPlayer(GameOptions gameOptions);
    }

    public class PlayerCreator : IPlayerCreator
    {
        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;

        public PlayerCreator(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
        }

        public Player CreateFirstPlayer(GameOptions gameOptions)
        {
            var position = Constants.BluePlayerStartIndex;
            var walls = Constants.WallsPerGame;
            var endPosition = Constants.BlueEndPositions;
            return gameOptions.color switch
            {
                StartColor.White => CreateBot(position, walls, endPosition),
                StartColor.Black => CreatePlayer(position, walls, endPosition),
                _ => gameOptions.gameMode == GameMode.BotVersusBot
                    ? CreateBot(position, walls, endPosition)
                    : CreatePlayer(position, walls, endPosition)
            };
        }

        public Player CreateSecondPlayer(GameOptions gameOptions)
        {
            var position = Constants.RedPlayerStartIndex;
            var walls = Constants.WallsPerGame;
            var endPosition = Constants.RedEndPositions;
            return gameOptions.color switch
            {
                StartColor.White => CreatePlayer(position, walls, endPosition),
                StartColor.Black => CreateBot(position, walls, endPosition),
                _ => gameOptions.gameMode == GameMode.VersusPlayer
                    ? CreatePlayer(position, walls, endPosition)
                    : CreateBot(position, walls, endPosition)
            };
        }

        public Player CreatePlayer(byte position, int walls, FieldMask endPosition)
        {
            return new Player(position, walls, endPosition, new ManualStrategy());
        }

        public Player CreateBot(byte position, int walls, FieldMask endPosition)
        {
            return new Player(position, walls, endPosition, GetStrategy());
        }

        private IMoveStrategy GetStrategy()
        {
            return new MonteCarloStrategy(moveProvider, wallProvider, search);
        }
    }
}
