namespace Quoridor.Logic
{
    using System;
    using System.Collections.Generic;
    using Tools;

    public class DijkstraSearch : ISearch
    {
        private readonly Direction[] directions = new Direction[4];

        private Dictionary<Direction, (int y, int x)> moveOffsets = new()
        {
            { Direction.Up, (-1, 0) },
            { Direction.Down, (1, 0) },
            { Direction.Left, (0, -1) },
            { Direction.Right, (0, 1) },
        };

        private readonly Dictionary<int, int> distances;
        private readonly Dictionary<int, int> prevNodes;
        private readonly List<int> queue;
        private readonly List<int> searced;
        private readonly Comparer comparer;
        private QuoridorModel model;

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
            SetDirections(direction);
            Initialize(position);
            return Search(out path);
        }

        private void SetDirections(Direction direction)
        {
            directions[0] = direction;
            directions[1] = Direction.Right;
            directions[2] = Direction.Left;
            directions[3] = direction.Opposite();
        }

        private void Initialize(int position)
        {
            for (var i = 0; i < QuoridorModel.UsedBitsAmount; i++)
            {
                distances[i] = int.MaxValue;
                prevNodes[i] = -1;
            }
            distances[position] = 0;
            queue.Add(position);
        }

        private bool Search(out Path path)
        {
            while (queue.Count > 0)
            {
                var position = queue[0];
                queue.RemoveAt(0);
                searced.Add(position);

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
            return directions[0] == Direction.Up ? i == 0 : i == QuoridorModel.BitboardSize - 1;
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
            foreach (var direction in directions)
            {
                if (TryGetPosition(position, direction, out var targetPosition))
                {
                    moves.Add(targetPosition);
                }
            }
            return moves;
        }

        private bool TryGetPosition(int position, Direction direction, out int targetPosition)
        {
            var (y, x) = Nested(position);
            var (yOffset, xOffset) = moveOffsets[direction];
            var targetX = x + 2 * xOffset;
            var targetY = y + 2 * yOffset;
            targetPosition = Flatten(targetY, targetX);
            if (!IsValid(targetY, targetX))
            {
                return false;
            }
            var moveMask = GetMask(y, x, yOffset, xOffset);
            return model.CanMove(moveMask);
        }

        private (int i, int j) Nested(int position)
        {
            var i = position / QuoridorModel.BitboardSize;
            var j = position % QuoridorModel.BitboardSize;
            return (i, j);
        }

        private int Flatten(int y, int x)
        {
            return x + y * QuoridorModel.BitboardSize;
        }

        private bool IsValid(int y, int x)
        {
            return model.IsInRange(y, x);
        }

        private FieldMask GetMask(int y, int x, int yOffset, int xOffset)
        {
            var moveMask = new FieldMask();
            moveMask.SetBit(y, x, true);
            moveMask.SetBit(y + yOffset, x + xOffset, true);
            moveMask.SetBit(y + 2 * yOffset, x + 2 * xOffset, true);
            return moveMask;
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
