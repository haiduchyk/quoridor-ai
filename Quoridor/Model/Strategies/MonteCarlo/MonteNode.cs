namespace Quoridor.Model.Strategies
{
    using System.Linq;
    using Moves;

    public class MonteNode
    {
        public bool IsPlayerMove => level % 2 == 1;

        public bool IsFullyExpanded => children.Sum(c => c.numberOfVisits) >= children.Length;

        public bool IsTerminal => children.Length == 0;

        public bool IsVisited => numberOfVisits > 0;

        public float WinRate => (float) wins / games;

        public int wins;
        public int games;

        public MonteNode parent;
        public readonly IMove move;
        public readonly int level;

        public MonteNode[] children;
        private int numberOfVisits;

        public MonteNode() : this(null, null, 0)
        {
        }

        public MonteNode(MonteNode parent, IMove move, int level)
        {
            this.parent = parent;
            this.move = move;
            this.level = level;
        }

        public void SetChild(MonteNode[] children)
        {
            this.children = children;
        }

        public MonteNode GetNextRoot(IMove lastMove)
        {
            foreach (var child in children)
            {
                if (child.move == lastMove)
                {
                    child.SetParentInNull();
                    return child;
                }
            }

            return null;
        }

        public void Update(int wins, int games)
        {
            this.wins += wins;
            this.games += games;
            this.numberOfVisits++;
        }

        private void SetParentInNull()
        {
            parent = null;
        }
    }
}