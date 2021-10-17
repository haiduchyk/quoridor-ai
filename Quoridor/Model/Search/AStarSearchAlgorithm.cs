namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class AStarSearchAlgorithm : SearchAlgorithm
    {
        private Heuristic heuristic;

        public AStarSearchAlgorithm(IMoveProvider moveProvider) : base(moveProvider)
        {
        }

        protected override IComparer<FieldMask> GetComparer()
        {
            return heuristic = new Heuristic(distances);
        }

        protected override void Prepare(FieldMask position)
        {
            base.Prepare(position);
            heuristic.SetEndPosition(endMask);
        }
    }
}
