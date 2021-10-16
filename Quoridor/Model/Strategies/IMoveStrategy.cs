namespace Quoridor.Model.Strategies
{
    using Moves;
    using Players;

    public interface IMoveStrategy
    {
        bool IsManual { get; }

        Move MakeMove(Field field, Player player, Player enemy);
    }
}
