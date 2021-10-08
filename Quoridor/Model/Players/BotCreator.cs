namespace Quoridor.Model
{
    using Players;
    using Strategies;

    public interface IBotCreator
    {
        Player CreateBotFor(FieldMask position, FieldMask endPosition, BotDifficulty botDifficulty);
    }

    public class BotCreator : IBotCreator
    {
        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;

        public BotCreator(IMoveProvider moveProvider, IWallProvider wallProvider)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
        }

        public Player CreateBotFor(FieldMask position, FieldMask endPosition, BotDifficulty botDifficulty)
        {
            return new Player(position, endPosition, Constants.WallsPerGame,
                new RandomMoveStrategy(moveProvider, wallProvider));
        }
    }
}
