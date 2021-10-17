namespace Quoridor.Model
{
    using Players;
    using Strategies;

    public interface IBotCreator
    {
        Player CreateBotFor(FieldMask position, string name, BotDifficulty botDifficulty);
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

        public Player CreateBotFor(FieldMask position, string name, BotDifficulty botDifficulty)
        {
            return new Player(position, Constants.WallsPerGame, name, GetStrategyFor(botDifficulty));
        }

        private IMoveStrategy GetStrategyFor(BotDifficulty botDifficulty)
        {
            return new RandomMoveStrategy(moveProvider, wallProvider);
        }
    }
}
