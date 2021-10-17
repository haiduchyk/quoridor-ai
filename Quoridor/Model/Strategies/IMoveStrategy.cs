namespace Quoridor.Model.Strategies
{
    using Moves;
    using Players;

    public interface IMoveStrategy
    {
        bool IsManual { get; }

        IMove MakeMove(Field field, Player player, Player enemy);
    }
}
