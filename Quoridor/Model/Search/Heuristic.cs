namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Strategies;

    public class Heuristic : IComparer<byte>
    {
        private readonly int[] distances;
        private readonly int[] blueHeuristic = new int[FieldMask.PlayerFieldArea];
        private readonly int[] redHeuristic = new int[FieldMask.PlayerFieldArea];
        private int[] heuristic;

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
