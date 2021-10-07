namespace Quoridor.Logic
{
    using System;
    using System.Collections.Generic;
    using Tools;

    public class DijkstraSearch : ISearch
    {
        private readonly Dictionary<Direction, (int y, int x)> moveOffsets = new()
        {
            { Direction.Up, (-1, 0) },
            { Direction.Down, (1, 0) },
            { Direction.Left, (0, -1) },
            { Direction.Right, (0, 1) },
        };

        private readonly Direction[] directions = new Direction[4];
        private readonly Dictionary<int, int> distances;
        private readonly Dictionary<int, int> prevNodes;
        private readonly SortedSet<int> queue;

        private Field field;

        public DijkstraSearch(Field field)
        {
            this.field = field;
            prevNodes = new Dictionary<int, int>(FieldMask.UsedBitsAmount);
            distances = new Dictionary<int, int>(FieldMask.UsedBitsAmount);
            var comparer = new Comparer(distances);
            queue = new SortedSet<int>(comparer);
        }

        public bool HasPath(int position, Direction direction, out Path path)
        {
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
            for (var i = 0; i < FieldMask.UsedBitsAmount; i++)
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
                var position = queue.Min;
                queue.Remove(position);

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
                        queue.Add(pos);
                    }
                }
            }

            path = default;
            return false;
        }

        private bool IsDestinationReached(int position)
        {
            var i = position / FieldMask.BitboardSize;
            return directions[0] == Direction.Up ? i == 0 : i == FieldMask.BitboardSize - 1;
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
            var (y, x) = field.Nested(position);
            var (yOffset, xOffset) = moveOffsets[direction];
            var targetX = x + 2 * xOffset;
            var targetY = y + 2 * yOffset;
            targetPosition = field.Flatten(targetY, targetX);
            if (!IsValid(targetY, targetX))
            {
                return false;
            }
            var moveMask = GetMask(y, x, yOffset, xOffset);
            return field.CanMove(ref moveMask);
        }

        
        private bool IsValid(int y, int x)
        {
            return FieldMask.IsInRange(y, x);
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
                return distances[x].CompareTo(distances[y]);
            }
        }
    }
}
