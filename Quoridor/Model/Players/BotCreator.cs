namespace Quoridor.Model
{
    using Players;
    using Strategies;

    public interface IBotCreator
    {
        Player CreateBotFor(FieldMask position, string name, BotDifficulty botDifficulty, FieldMask endPosition);
    }

    public class BotCreator : IBotCreator
    {
        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;

        public BotCreator(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
        }

        public Player CreateBotFor(FieldMask position, string name, BotDifficulty botDifficulty, FieldMask endPosition)
        {
            return new Player(position, Constants.WallsPerGame, name, GetStrategyFor(botDifficulty), endPosition);
        }

        private IMoveStrategy GetStrategyFor(BotDifficulty botDifficulty)
        {
            return new MonteCarloStrategy(moveProvider, wallProvider, search);
        }
    }
}
