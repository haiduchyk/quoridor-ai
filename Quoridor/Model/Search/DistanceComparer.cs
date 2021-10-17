namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class DistanceComparer : IComparer<FieldMask>
    {
        private readonly Dictionary<FieldMask, int> distances;

        public DistanceComparer(Dictionary<FieldMask, int> distances)
        {
            this.distances = distances;
        }

        public int Compare(FieldMask x, FieldMask y)
        {
            return distances[x].CompareTo(distances[y]);
        }
    }
}
