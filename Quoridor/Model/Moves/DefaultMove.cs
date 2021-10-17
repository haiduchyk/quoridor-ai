namespace Quoridor.Model.Moves
{
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
    }
}
