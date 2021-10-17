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
            BluePlayer = CreateFirstPlayer();
            RedPlayer = CreateSecondPlayer(gameOptions, botCreator);
        }

        private Player CreateFirstPlayer()
        {
            var position = Constants.BluePlayerPosition;
            var name = Constants.BluePlayerName;
            return new Player(position, Constants.WallsPerGame, name, new ManualStrategy());
        }

        private Player CreateSecondPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.RedPlayerPosition;
            var name = Constants.RedPlayerName;
            return gameOptions.gameMode == GameMode.VersusPlayer
                ? new Player(position, Constants.WallsPerGame, name, new ManualStrategy())
                : botCreator.CreateBotFor(position, name, gameOptions.botDifficulty);
        }

        public bool HasFinished()
        {
            return BluePlayer.Position.And(in Constants.BlueEndPositions).IsNotZero() ||
                   RedPlayer.Position.And(in Constants.RedEndPositions).IsNotZero();
        }
    }
}
