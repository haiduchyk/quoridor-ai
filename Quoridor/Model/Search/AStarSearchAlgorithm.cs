namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;

    public class AStarSearchAlgorithm : SearchAlgorithm
    {
        private Heuristic heuristic;

        public AStarSearchAlgorithm(IMoveProvider moveProvider, PathRetriever pathRetriever)
            : base(moveProvider, pathRetriever)
        {
        }

        protected override IComparer<byte> GetComparer()
        {
            return heuristic = new Heuristic(distances);
        }

        protected override void Prepare(Player player, in byte position)
        {
            base.Prepare(player, position);
            heuristic.SetEndPosition(player.EndDownIndex);
        }
    }
}
