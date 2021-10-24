namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class DijkstraSearchAlgorithm : SearchAlgorithm
    {
        public DijkstraSearchAlgorithm(IMoveProvider moveProvider, PathWithWallsRetriever pathRetriever)
            : base(moveProvider, pathRetriever)
        {
        }

        protected override IComparer<FieldMask> GetComparer()
        {
            return new DistanceComparer(distances);
        }
    }
}
