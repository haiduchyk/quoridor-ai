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
            
            var endUpIndex = PlayerConstants.EndBlueUpIndexIncluding;
            var endDownIndex = PlayerConstants.EndBlueDownIndexIncluding;
            
            
            return gameOptions.color switch
            {
                StartColor.White => CreateBot(position, walls, endUpIndex, endDownIndex),
                StartColor.Black => CreatePlayer(position, walls, endUpIndex, endDownIndex),
                _ => gameOptions.gameMode == GameMode.BotVersusBot
                    ? CreateBot(position, walls, endUpIndex, endDownIndex)
                    : CreatePlayer(position, walls, endUpIndex, endDownIndex)
            };
        }

        public Player CreateSecondPlayer(GameOptions gameOptions)
        {
            var position = Constants.RedPlayerStartIndex;
            var walls = Constants.WallsPerGame;
            var endUpIndex = PlayerConstants.EndRedUpIndexIncluding;
            var endDownIndex = PlayerConstants.EndRedDownIndexIncluding;

            return gameOptions.color switch
            {
                StartColor.White => CreatePlayer(position, walls, endUpIndex, endDownIndex),
                StartColor.Black => CreateBot(position, walls, endUpIndex, endDownIndex),
                _ => gameOptions.gameMode == GameMode.VersusPlayer
                    ? CreatePlayer(position, walls, endUpIndex, endDownIndex)
                    : CreateBot(position, walls, endUpIndex, endDownIndex)
            };
        }

        public Player CreatePlayer(byte position, int walls, byte endUpIndex, byte endDownIndex)
        {
            return new Player(position, walls, endUpIndex, endDownIndex, new ManualStrategy());
        }

        public Player CreateBot(byte position, int walls, byte endUpIndex, byte endDownIndex)
        {
            return new Player(position, walls, endUpIndex, endDownIndex, GetStrategy());
        }

        private IMoveStrategy GetStrategy()
        {
            return new MonteCarloStrategy(moveProvider, wallProvider, search);
        }
    }
}