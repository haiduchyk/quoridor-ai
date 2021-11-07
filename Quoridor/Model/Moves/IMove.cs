namespace Quoridor.Model.Moves
{
    using Players;

    public interface IMove
    {
        bool IsMove { get; }

        bool IsValid();

        void Execute();

        void ExecuteForSimulation();

        void Apply(Field field, Player player);

        ref readonly byte Id { get; }

        public void Log();
    }
}
