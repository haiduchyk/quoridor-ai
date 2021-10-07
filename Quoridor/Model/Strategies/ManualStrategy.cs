namespace Quoridor.Model.Strategies
{
    public class ManualStrategy : IMoveStrategy
    {
        public bool IsManual => true;

        public FieldMask MakeMove(Field field, FieldMask playerMask)
        {
            return new FieldMask();
        }
    }
}
