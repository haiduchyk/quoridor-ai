namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Strategies;

    public class Heuristic : IComparer<byte>
    {
        private readonly Dictionary<byte, int> distances;
        private readonly Dictionary<(byte, byte), int> heuristic = new();

        private byte endPosition;

        public Heuristic(Dictionary<byte, int> distances)
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
                    heuristic[(PlayerConstants.EndBlueDownIndexIncluding, index)] = i;
                    heuristic[(PlayerConstants.EndRedDownIndexIncluding, index)] = FieldMask.PlayerFieldSize - 1 - i;
                }
            }
        }

        public void SetEndPosition(in byte endPosition)
        {
            this.endPosition = endPosition;
        }

        public int Compare(byte first, byte second)
        {
            return distances[first] + heuristic[(endPosition, first)] -
                   (distances[second] + heuristic[(endPosition, second)]);
        }
    }
}
