namespace Quoridor.Model
{
    using Players;
    using Game;
    using Strategies;

    public interface IBotCreator
    {
        Player CreateBotFor(FieldMask position, BotDifficulty botDifficulty);
    }

    public class BotCreator : IBotCreator
    {
        private readonly IMoveProvider moveProvider;

        public BotCreator(IMoveProvider moveProvider)
        {
            this.moveProvider = moveProvider;
        }

        public Player CreateBotFor(FieldMask position, BotDifficulty botDifficulty)
        {
            return new Player(position, Constants.WallsPerGame, new RandomMoveStrategy(moveProvider));
        }
    }
}
