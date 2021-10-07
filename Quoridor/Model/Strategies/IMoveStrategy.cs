namespace Quoridor.Model.Strategies
{
    public interface IMoveStrategy
    {
        bool IsManual { get; }

        FieldMask MakeMove(Field field, FieldMask playerMask);
    }
}
