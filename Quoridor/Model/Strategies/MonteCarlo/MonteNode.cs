namespace Quoridor.Model.Strategies
{
    using System.Linq;
    using Moves;

    public class MonteNode
    {
        public bool IsPlayerMove => level % 2 == 0;

        public bool IsFullyExpanded => children.All(n => n.IsVisited);

        public bool IsVisited => games > 0;

        public double WinRate => (double)wins / games;

        public int wins;
        public int games;

        public readonly MonteNode parent;
        public readonly IMove move;
        public readonly int level;

        public MonteNode[] children;

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

        public void Update(int result)
        {
            wins += result;
            games++;
        }
    }
}
