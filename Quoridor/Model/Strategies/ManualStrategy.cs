namespace Quoridor.Model.Strategies
{
    using Moves;
    using Players;

    public class ManualStrategy : IMoveStrategy
    {
        public bool IsManual => true;

        public Move MakeMove(Field field, Player player, Player enemy)
        {
            return new DefaultMove(field, player, new FieldMask());
        }
    }
}
