namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;

    public abstract class SearchAlgorithm : ISearch
    {
        protected readonly Dictionary<FieldMask, int> distances = new(81);

        private readonly FieldMask[] possiblePositions = new FieldMask[81];
        private readonly Dictionary<FieldMask, (FieldMask mask, bool isSimple)> prevNodes = new(81);
        private readonly PriorityQueue<FieldMask> queue;

        private readonly IMoveProvider moveProvider;
        private readonly PathWithWallsRetriever pathRetriever;

        private Field field;

        public SearchAlgorithm(IMoveProvider moveProvider, PathWithWallsRetriever pathRetriever)
        {
            this.moveProvider = moveProvider;
            this.pathRetriever = pathRetriever;
            var comparer = GetComparer();
            queue = new PriorityQueue<FieldMask>(comparer);
            FindPossiblePositions();
        }

        protected abstract IComparer<FieldMask> GetComparer();

        private void FindPossiblePositions()
        {
            var count = 0;
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var mask = new FieldMask();
                        mask.SetBit(y, x, true);
                        possiblePositions[count] = mask;
                        count++;
                    }
                }
            }
        }

        public bool HasPath(Field field, Player player, in FieldMask position, out FieldMask path)
        {
            this.field = field;
            Prepare(player, in position);
            return Search(player, out path);
        }

        protected virtual void Prepare(Player player, in FieldMask position)
        {
            for (var i = 0; i < 81; i++)
            {
                var pos = possiblePositions[i];
                distances[pos] = int.MaxValue;
                prevNodes[pos] = (Constants.EmptyField, false);
            }

            distances[position] = 0;
            queue.Clear();
            queue.Enqueue(position);
        }

        private bool Search(Player player, out FieldMask path)
        {
            while (queue.Count > 0)
            {
                var position = queue.Dequeue();

                if (IsDestinationReached(player, position))
                {
                    path = pathRetriever.RetrievePath(position, prevNodes, player.Enemy.Position);
                    return true;
                }

                var traversedDistance = distances[position];

                var (masks, isSimple) = GetPossibleMoves(player, in position);

                foreach (var pos in masks)
                {
                    var distance = traversedDistance + 1;
                    if (distance < distances[pos])
                    {
                        distances[pos] = distance;
                        prevNodes[pos] = (position, isSimple: isSimple);
                        queue.Enqueue(pos);
                    }
                }
            }

            path = Constants.EmptyField;
            return false;
        }

        private bool IsDestinationReached(Player player, FieldMask position)
        {
            return player.IsEndPosition(position);
        }

        private (FieldMask[] masks, bool isSimple) GetPossibleMoves(Player player, in FieldMask position)
        {
            return moveProvider.GetAvailableMovesWithType(field, in position, in player.Enemy.Position);
        }
    }
}
