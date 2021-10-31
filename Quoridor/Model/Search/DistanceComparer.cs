namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class DistanceComparer : IComparer<byte>
    {
        private readonly Dictionary<byte, int> distances;

        public DistanceComparer(Dictionary<byte, int> distances)
        {
            this.distances = distances;
        }

        public int Compare(byte x, byte y)
        {
            return distances[x] - distances[y];
        }
    }
}
