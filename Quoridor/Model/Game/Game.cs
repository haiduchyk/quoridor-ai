namespace Quoridor.Model
{
    using Players;
    using Strategies;

    public class Game
    {
        public Field Field { get; }

        public Player BluePlayer { get; }

        public Player RedPlayer { get; }

        public bool IsFinished => BluePlayer.HasReachedEnd() || RedPlayer.HasReachedEnd();

        public Game(GameOptions gameOptions, IBotCreator botCreator)
        {
            Field = new Field(FieldMask.BitboardSize);
            BluePlayer = CreateFirstPlayer(gameOptions, botCreator);
            RedPlayer = CreateSecondPlayer(gameOptions, botCreator);
        }

        private Player CreateSecondPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.RedPlayerPosition;
            var endPosition = new FieldMask();
            return gameOptions.gameMode == GameMode.VersusPlayer
                ? new Player(position, endPosition, Constants.WallsPerGame, new ManualStrategy())
                : botCreator.CreateBotFor(position, endPosition, gameOptions.botDifficulty);
        }
        
        private Player CreateFirstPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.BluePlayerPosition;
            var endPosition = new FieldMask();
            return gameOptions.gameMode == GameMode.VersusPlayer
                ? new Player(position, endPosition, Constants.WallsPerGame, new ManualStrategy())
                : botCreator.CreateBotFor(position, endPosition, gameOptions.botDifficulty);
        }
    }
}
