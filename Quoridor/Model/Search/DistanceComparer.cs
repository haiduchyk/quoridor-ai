namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class DistanceComparer : IComparer<byte>
    {
        private readonly int[] distances;

        public DistanceComparer(int[] distances)
        {
            this.distances = distances;
        }

        public int Compare(byte x, byte y)
        {
            return distances[x] - distances[y];
        }
    }
}
