namespace Quoridor.Model.Moves
{
    using Players;

    public interface IMove
    {
        bool IsValid();

        void Execute();

        void Undo();

        void Apply(Field field, Player player);
    }
}
