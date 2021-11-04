namespace Quoridor.Model.Strategies
{
    using Moves;
    using Players;

    public interface IMoveStrategy
    {
        bool IsManual { get; }

        IMove FindMove(Field field, Player player, IMove lastMove);
    }
}
