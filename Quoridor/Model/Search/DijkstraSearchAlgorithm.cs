namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class DijkstraSearchAlgorithm : SearchAlgorithm
    {
        public DijkstraSearchAlgorithm(IMoveProvider moveProvider, PathRetriever pathRetriever)
            : base(moveProvider, pathRetriever)
        {
        }

        protected override IComparer<byte> GetComparer()
        {
            return new DistanceComparer(distances);
        }
        
        public override ISearch Copy()
        {
            return new DijkstraSearchAlgorithm(moveProvider, pathRetriever);
        }
    }
}
