namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;
    using Players;

    public abstract class SearchAlgorithm : ISearch
    {
        private readonly Dictionary<(FieldMask walls, byte player, byte enemy, byte pos), FieldMask> cached = new();

        protected readonly int[] distances = new int[81];

        private readonly byte[] possiblePositions = new byte[81];
        private readonly Dictionary<byte, (byte mask, bool isSimple)> prevNodes = new(81);
        private readonly PriorityQueue<byte> queue;

        private readonly IMoveProvider moveProvider;
        private readonly PathRetriever pathRetriever;

        private Field field;

        protected SearchAlgorithm(IMoveProvider moveProvider, PathRetriever pathRetriever)
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
                possiblePositions[i] = (byte)i;
            }
        }

        public bool HasPath(Field field, Player player, in byte position)
        {
            this.field = field;
            Prepare(player, in position);
            return Search(player, false, out _);
        }

        public bool TryFindPath(Field field, Player player, in byte position, out FieldMask path)
        {
            this.field = field;
            Prepare(player, in position);
            return Search(player, true, out path);
        }

        public void UpdatePathForPlayers(Field field, Player player)
        {
            this.field = field;
            UpdatePathFor(player);
            UpdatePathFor(player.Enemy);
        }

        public void UpdatePathFor(Player player)
        {
            Prepare(player, in player.Position);
            var key = (field.Walls, player.Position, player.Enemy.Position, player.Position);
            if (cached.TryGetValue(key, out var path))
            {
                player.SetPath(path);
                return;
            }
            var result = Search(player, true, out path);
            player.SetPath(path);
            cached[key] = path;
        }

        protected virtual void Prepare(Player player, in byte position)
        {
            for (var i = 0; i < 81; i++)
            {
                var pos = possiblePositions[i];
                distances[pos] = int.MaxValue;
                prevNodes[pos] = (Constants.EmptyIndex, default);
            }

            distances[position] = 0;
            queue.Clear();
            queue.Enqueue(position);
        }

        private bool Search(Player player, bool retrievePath, out FieldMask path)
        {
            while (queue.Count > 0)
            {
                var position = queue.Dequeue();

                if (IsDestinationReached(player, position))
                {
                    path = retrievePath
                        ? pathRetriever.RetrievePath(position, prevNodes, player.Enemy.Position)
                        : Constants.EmptyField;
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
                        prevNodes[pos] = (position, isSimple);
                        queue.Enqueue(pos);
                    }
                }
            }

            path = Constants.EmptyField;
            return false;
        }

        private bool IsDestinationReached(Player player, byte position)
        {
            return player.IsEndPosition(position);
        }

        private (byte[] indexes, bool isSimple) GetPossibleMoves(Player player, in byte position)
        {
            return moveProvider.GetAvailableMovesWithType(field, in position, in player.Enemy.Position);
        }
    }
}
