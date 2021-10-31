namespace Quoridor.Model.Moves
{
    using Players;

    public interface IMove
    {
        bool IsValid();

        void Execute();

        void Apply(Field field, Player player);

        ref readonly byte Id { get; }

        public void Log()
        {
            Id.ToString().Log();
        }
    }
}
