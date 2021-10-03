namespace Quoridor.Logic
{
    using System;
    using System.Collections.Generic;
    using Tools;

    public class DijkstraSearch : ISearch
    {
        private Direction dominantDirection;
        private QuoridorModel model;

        private readonly Dictionary<int, int> distances;
        private readonly Dictionary<int, int> prevNodes;
        private readonly List<int> queue;
        private readonly List<int> searced;
        private readonly Comparer comparer;

        public DijkstraSearch()
        {
            prevNodes = new Dictionary<int, int>(QuoridorModel.UsedBitsAmount);
            distances = new Dictionary<int, int>(QuoridorModel.UsedBitsAmount);
            queue = new List<int>(QuoridorModel.UsedBitsAmount);
            searced = new List<int>(QuoridorModel.UsedBitsAmount);
            comparer = new Comparer(distances);
        }

        public bool HasPath(QuoridorModel model, int position, Direction direction, out Path path)
        {
            this.model = model;
            dominantDirection = direction;
            queue.Clear();
            for (var i = 0; i < QuoridorModel.UsedBitsAmount; i++)
            {
                distances[i] = int.MaxValue;
                prevNodes[i] = -1;
            }
            distances[position] = 0;
            queue.Add(position);
            return Search(out path);
        }

        private bool Search(out Path path)
        {
            while (queue.Count > 0)
            {
                var position = queue[0];
                queue.RemoveAt(0);
                searced.Add(position);
                var (i, j) = Nested(position);
                if (i == 6)
                {
                    Console.WriteLine($"s");
                }
                
                if (IsDestinationReached(position))
                {
                    path = RetrievePath(position);
                    return true;
                }

                var traversedDistance = distances[position];
                foreach (var pos in GetPossibleMoves(position))
                {
                    var distance = traversedDistance + 1;
                    if (distance < distances[pos])
                    {
                        distances[pos] = distance;
                        prevNodes[pos] = position;
                        queue.Remove(pos);
                        queue.Add(pos);
                        queue.Sort(comparer);
                    }
                }
            }

            path = default;
            return false;
        }

        private bool IsDestinationReached(int position)
        {
            var i = position / QuoridorModel.BitboardSize;
            return dominantDirection == Direction.Up ? i == 0 : i == QuoridorModel.BitboardSize - 1;
        }

        private (int i, int j) Nested(int position)
        {
            var i = position / QuoridorModel.BitboardSize;
            var j = position % QuoridorModel.BitboardSize;
            return (i, j);
        }

        private Path RetrievePath(int position)
        {
            var path = new Path
            {
                nodes = new List<int>()
            };
            while (position != -1)
            {
                path.nodes.Insert(0, position);
                position = prevNodes[position];
            }
            return path;
        }

        private List<int> GetPossibleMoves(int position)
        {
            var moves = new List<int>(4);
            if (TryGetPosition(position, dominantDirection, out var targetPosition))
            {
                moves.Add(targetPosition);
            }
            if (TryGetPosition(position, Direction.Right, out targetPosition))
            {
                moves.Add(targetPosition);
            }
            if (TryGetPosition(position, Direction.Left, out targetPosition))
            {
                moves.Add(targetPosition);
            }
            if (TryGetPosition(position, dominantDirection.Opposite(), out targetPosition))
            {
                moves.Add(targetPosition);
            }
            return moves;
        }

        private bool TryGetPosition(int position, Direction direction, out int targetPosition)
        {
            var offset = (int)direction;
            targetPosition = position + 2 * offset;
            if (!IsValid(targetPosition))
            {
                return false;
            }
            var moveMask = GetMask(position, offset);
            return model.CanMove(moveMask);
        }

        private FieldMask GetMask(int position, int offset)
        {
            var moveMask = new FieldMask();
            moveMask.SetBit(position, true);
            moveMask.SetBit(position + offset, true);
            moveMask.SetBit(position + 2 * offset, true);
            return moveMask;
        }

        private bool IsValid(int position)
        {
            return model.IsInRange(position);
        }

        private class Comparer : IComparer<int>
        {
            private readonly Dictionary<int, int> distances;

            public Comparer(Dictionary<int, int> distances)
            {
                this.distances = distances;
            }

            public int Compare(int x, int y)
            {
                return distances[x] - distances[y];
            }
        }
    }
}
