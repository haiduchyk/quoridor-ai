namespace Quoridor.Model.Moves
{
    using Players;

    public class DefaultMove : IMove
    {
        public ref readonly byte Id => throw new System.NotImplementedException();

        public bool IsValid()
        {
            return false;
        }

        public void Execute()
        {
        }

        public void Apply(Field field, Player player)
        {
        }

    }
}
