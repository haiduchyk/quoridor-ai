namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;

    public class AStarSearchAlgorithm : SearchAlgorithm
    {
        private Heuristic heuristic;

        public AStarSearchAlgorithm(IMoveProvider moveProvider, PathWithWallsRetriever pathRetriever) 
            : base(moveProvider, pathRetriever)
        {
        }

        protected override IComparer<FieldMask> GetComparer()
        {
            return heuristic = new Heuristic(Distances);
        }

        protected override void Prepare(Player player, in FieldMask position)
        {
            base.Prepare(player, position);
            heuristic.SetEndPosition(player.EndPosition);
        }
    }
}
