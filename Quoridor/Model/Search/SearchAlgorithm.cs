namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;

    public abstract class SearchAlgorithm : ISearch
    {
        protected readonly Dictionary<byte, int> Distances = new(81);

        private readonly byte[] possiblePositions = new byte[81];
        private readonly Dictionary<byte, (byte mask, bool isSimple)> prevNodes = new(81);
        private readonly PriorityQueue<byte> queue;

        private readonly IMoveProvider moveProvider;
        private readonly PathWithWallsRetriever pathRetriever;

        private Field field;

        public SearchAlgorithm(IMoveProvider moveProvider, PathWithWallsRetriever pathRetriever)
        {
            this.moveProvider = moveProvider;
            this.pathRetriever = pathRetriever;
            var comparer = GetComparer();
            queue = new PriorityQueue<byte>(comparer);
            FindPossiblePositions();
        }

        protected abstract IComparer<byte> GetComparer();

        private void FindPossiblePositions()
        {
            for (var i = 0; i < FieldMask.PlayerFieldArea; i++)
            {
                possiblePositions[i] = (byte) i;
            }
        }

        public bool HasPath(Field field, Player player, in byte position, out FieldMask path)
        {
            this.field = field;
            Prepare(player, in position);
            var result = Search(player, out path);
            player.CurrentPath = path;
            return result;
        }

        protected virtual void Prepare(Player player, in byte position)
        {
            for (var i = 0; i < 81; i++)
            {
                var pos = possiblePositions[i];
                Distances[pos] = int.MaxValue;
                prevNodes[pos] = (Constants.EmptyIndex, default);
            }

            Distances[position] = 0;
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

                var traversedDistance = Distances[position];

                var (masks, isSimple) = GetPossibleMoves(player, in position);

                foreach (var pos in masks)
                {
                    var distance = traversedDistance + 1;
                    if (distance < Distances[pos])
                    {
                        Distances[pos] = distance;
                        prevNodes[pos] = (position, isSimple);
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

        private (byte[] indexes, bool isSimple) GetPossibleMoves(Player player, in byte position)
        {
            return moveProvider.GetAvailableMovesWithType(field, in position, in player.Enemy.Position);
        }
    }
}