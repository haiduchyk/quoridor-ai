namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Strategies;

    public class Heuristic : IComparer<byte>
    {
        private readonly int[] distances;
        private readonly Dictionary<byte, int> blueHeuristic = new();
        private readonly Dictionary<byte, int> redHeuristic = new();
        private Dictionary<byte, int> heuristic;

        public Heuristic(int[] distances)
        {
            this.distances = distances;
            InitializeHeuristic();
        }

        private void InitializeHeuristic()
        {
            for (var i = 0; i < FieldMask.PlayerFieldSize; i++)
            {
                for (var j = 0; j < FieldMask.PlayerFieldSize; j++)
                {
                    var index = (byte)(i * FieldMask.PlayerFieldSize + j);
                    blueHeuristic[index] = i;
                    redHeuristic[index] = FieldMask.PlayerFieldSize - 1 - i;
                }
            }
        }

        public void SetEndPosition(in byte endPosition)
        {
            heuristic = endPosition == PlayerConstants.EndBlueDownIndexIncluding ? blueHeuristic : redHeuristic;
        }

        public int Compare(byte first, byte second)
        {
            return distances[first] + heuristic[first] -
                   (distances[second] + heuristic[second]);
        }
    }
}
