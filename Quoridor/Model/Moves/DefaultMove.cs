namespace Quoridor.Model.Moves
{
    using Players;

    public class DefaultMove : IMove
    {
        public bool IsValid()
        {
            return false;
        }

        public void Execute()
        {
        }

        public void Undo()
        {
        }

        public void Apply(Field field, Player player, Player enemy)
        {
        }
    }
}
