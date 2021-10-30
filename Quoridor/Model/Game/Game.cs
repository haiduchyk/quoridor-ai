namespace Quoridor.Model
{
    using Players;
    using Strategies;

    public class Game
    {
        public Field Field { get; }

        public Player BluePlayer { get; }

        public Player RedPlayer { get; }

        public Game(GameOptions gameOptions, IBotCreator botCreator, IWallProvider wallProvider)
        {
            Field = new Field(FieldMask.BitboardSize);
            Field.ValidWalls.AddRange(wallProvider.GetAllMoves());
            BluePlayer = CreateFirstPlayer();
            RedPlayer = CreateSecondPlayer(gameOptions, botCreator);
            BluePlayer.SetEnemy(RedPlayer);
            RedPlayer.SetEnemy(BluePlayer);
        }

        private Player CreateFirstPlayer()
        {
            var position = Constants.BluePlayerPosition;
            var name = Constants.BluePlayerName;
            var endPosition = Constants.BlueEndPositions;
            return new Player(position, Constants.WallsPerGame, name, new ManualStrategy(), endPosition);
        }

        private Player CreateSecondPlayer(GameOptions gameOptions, IBotCreator botCreator)
        {
            var position = Constants.RedPlayerPosition;
            var name = Constants.RedPlayerName;
            var endPosition = Constants.RedEndPositions;
            return gameOptions.gameMode == GameMode.VersusPlayer
                ? new Player(position, Constants.WallsPerGame, name, new ManualStrategy(), endPosition)
                : botCreator.CreateBotFor(position, name, gameOptions.botDifficulty, endPosition);
        }

        public bool HasFinished()
        {
            return BluePlayer.Position.And(in Constants.BlueEndPositions).IsNotZero() ||
                   RedPlayer.Position.And(in Constants.RedEndPositions).IsNotZero();
        }
    }
}
