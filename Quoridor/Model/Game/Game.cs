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
            BluePlayer = new Player(Constants.BluePlayerPosition, Constants.BlueEndPositions, Constants.WallsPerGame,
                new ManualStrategy());
            RedPlayer = CreateSecondPlayer(gameOptions, botCreator);
        }

        private Player CreateSecondPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.RedPlayerPosition;
            var endPosition = Constants.RedEndPositions;
            return gameOptions.gameMode == GameMode.VersusPlayer
                ? new Player(position, endPosition, Constants.WallsPerGame, new ManualStrategy())
                : botCreator.CreateBotFor(position, endPosition, gameOptions.botDifficulty);
        }
    }
}
