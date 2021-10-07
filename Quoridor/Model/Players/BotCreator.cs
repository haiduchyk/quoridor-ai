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

        public BotCreator(IMoveProvider moveProvider)
        {
            this.moveProvider = moveProvider;
        }

        public Player CreateBotFor(FieldMask position, FieldMask endPosition, BotDifficulty botDifficulty)
        {
            return new Player(position, endPosition, Constants.WallsPerGame, new RandomMoveStrategy(moveProvider));
        }
    }
}
