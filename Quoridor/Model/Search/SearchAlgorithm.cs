namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;
    using Players;

    public abstract class SearchAlgorithm : ISearch
    {
        protected readonly Dictionary<byte, int> distances = new(81);

        private readonly byte[] possiblePositions = new byte[81];
        private readonly Dictionary<byte, (byte mask, bool isSimple)> prevNodes = new(81);
        private readonly PriorityQueue<byte> queue;

        private readonly IMoveProvider moveProvider;
        private readonly PathRetriever pathRetriever;

        private Field field;
        
        // TODO please add DI
        public static ISearch Instance;
        
        public SearchAlgorithm(IMoveProvider moveProvider, PathRetriever pathRetriever)
        {
            // TODO please add DI
            Instance = this;
            
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
            return result;
        }

        public void UpdatePathForPlayers(Field field, Player player)
        {
            this.field = field;
            Prepare(player, in player.Position);
            var result = Search(player, out var path);
            if (!result)
            {
                Console.WriteLine($" What");
            }
            player.CurrentPath = path;


            var enemy = player.Enemy;
            Prepare(enemy, in enemy.Position);
            result = Search(enemy, out path);
            if (!result)
            {
                Console.WriteLine($" What");
            }
            enemy.CurrentPath = path;
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
