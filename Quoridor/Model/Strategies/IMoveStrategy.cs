namespace Quoridor.Model.Strategies
{
    public interface IMoveStrategy
    {
        FieldMask MakeMove(FieldMask playerMask);
    }
}
