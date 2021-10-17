namespace Quoridor.Model.Moves
{
    public interface IMove
    {
        public abstract bool IsValid();

        public abstract void Execute();

        public abstract void Undo();
    }
}
