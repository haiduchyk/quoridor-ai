namespace Quoridor.Model
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
            BluePlayer = CreateFirstPlayer(gameOptions, botCreator);
            RedPlayer = CreateSecondPlayer(gameOptions, botCreator);
        }

        private Player CreateFirstPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.BluePlayerPosition;
            return gameOptions.gameMode == GameMode.VersusPlayer
                ? new Player(position, Constants.WallsPerGame, new ManualStrategy())
                : botCreator.CreateBotFor(position, gameOptions.botDifficulty);
        }

        private Player CreateSecondPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.RedPlayerPosition;
            return gameOptions.gameMode == GameMode.VersusPlayer
                ? new Player(position, Constants.WallsPerGame, new ManualStrategy())
                : botCreator.CreateBotFor(position, gameOptions.botDifficulty);
        }

        public bool HasFinished()
        {
            return BluePlayer.Position.And(in Constants.BlueEndPositions).IsNotZero() &&
                   RedPlayer.Position.And(in Constants.RedEndPositions).IsNotZero();
        }
    }
}
