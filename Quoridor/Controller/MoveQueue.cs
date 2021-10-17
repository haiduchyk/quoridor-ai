namespace Quoridor.Controller
{
    using System.Collections.Generic;
    using Model.Moves;

    public class MoveQueue
    {
        private readonly Stack<IMove> stack = new();

        public void Add(IMove move)
        {
            stack.Push(move);
        }

        public void Undo()
        {
            if (stack.Count != 0)
            {
                var move = stack.Pop();
                move.Undo();
            }
        }

        public void Clear()
        {
            stack.Clear();
        }
    }
}
