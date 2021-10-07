namespace Quoridor.Logic.Strategies
{
    public interface IMoveStrategy
    {
        FieldMask MakeMove(FieldMask playerMask);
    }
}
