namespace Quoridor.Model.Game
{
    using Players;
    using Strategies;

    public class Game
    {
        public Field Field { get; }

        public Player BluePlayer { get; }

        public Player RedPlayer { get; }

        public Game(GameOptions gameOptions, IBotCreator botCreator)
        {
            Field = new Field(FieldMask.BitboardSize);
            BluePlayer = new Player(Constants.BlueCharacterPosition, Constants.WallsPerGame, new ManualStrategy());
            RedPlayer = CreateSecondPlayer(gameOptions, botCreator);
        }

        private Player CreateSecondPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.RedCharacterPosition;
            return gameOptions.gameMode == GameMode.VersusPlayer ?
                new Player(position, Constants.WallsPerGame, new ManualStrategy()) :
                botCreator.CreateBotFor(position, gameOptions.botDifficulty);
        }
    }
}
