namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class Heuristic : IComparer<FieldMask>
    {
        private readonly Dictionary<FieldMask, int> distances;
        private readonly Dictionary<FieldMask, FieldMask> rows = new();
        private readonly Dictionary<FieldMask, Dictionary<FieldMask, int>> heuristic = new();

        private FieldMask endPosition;

        public Heuristic(Dictionary<FieldMask, int> distances)
        {
            this.distances = distances;
            InitializeHeuristic();
        }

        private void InitializeHeuristic()
        {
            heuristic[Constants.BlueEndPositions] = new Dictionary<FieldMask, int>();
            heuristic[Constants.RedEndPositions] = new Dictionary<FieldMask, int>();
            for (var i = 0; i < FieldMask.BitboardSize; i += 2)
            {
                var rowMask = new FieldMask();
                rowMask.SetBit(i, 0, true);
                for (var j = 0; j < FieldMask.BitboardSize; j += 2)
                {
                    var mask = new FieldMask();
                    mask.SetBit(i, j, true);
                    rows[mask] = rowMask;
                }
                heuristic[Constants.BlueEndPositions][rowMask] = i;
                heuristic[Constants.RedEndPositions][rowMask] = 8 - i;
            }
        }

        public void SetEndPosition(FieldMask endPosition)
        {
            this.endPosition = endPosition;
        }

        public int Compare(FieldMask first, FieldMask second)
        {
            // Actual shit
            // var firstHeuristic = distances[first] + heuristic[endPosition][rows[first]];
            // var secondHeuristic = distances[second] + heuristic[endPosition][rows[second]];
            return distances[first] + heuristic[endPosition][rows[first]] - distances[second] + heuristic[endPosition][rows[second]];
        }
    }
}
