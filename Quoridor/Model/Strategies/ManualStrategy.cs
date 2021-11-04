namespace Quoridor.Model.Strategies
{
    using Moves;
    using Players;

    public class ManualStrategy : IMoveStrategy
    {
        public bool IsManual => true;

        public IMove FindMove(Field field, Player player, IMove lastMove)
        {
            return new DefaultMove();
        }
    }
}
