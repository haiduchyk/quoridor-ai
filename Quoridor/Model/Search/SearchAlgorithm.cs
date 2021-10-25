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

        protected FieldMask endMask;
        private Game game;
        private Field field;
        private Player enemy;

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

        public void Initialize(Game game)
        {
            this.game = game;
        }

        public bool HasPath(Field field, Player player, FieldMask position, out FieldMask path)
        {
            this.field = field;
            endMask = GetEndMask(player);
            enemy = GetEnemy(player);
            Prepare(position);
            return Search(out path);
        }

        private FieldMask GetEndMask(Player player)
        {
            return player == game.BluePlayer ? Constants.BlueEndPositions : Constants.RedEndPositions;
        }

        private Player GetEnemy(Player player)
        {
            return player == game.BluePlayer ? game.RedPlayer : game.BluePlayer;
        }

        protected virtual void Prepare(FieldMask position)
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

        private bool Search(out FieldMask path)
        {
            while (queue.Count > 0)
            {
                var position = queue.Dequeue();

                if (IsDestinationReached(position))
                {
                    path = pathRetriever.RetrievePath(position, prevNodes, enemy.Position);
                    return true;
                }

                var traversedDistance = distances[position];

                var moves = GetPossibleMoves(position);
                
                foreach (var pos in moves.masks)
                {
                    var distance = traversedDistance + 1;
                    if (distance < distances[pos])
                    {
                        distances[pos] = distance;
                        prevNodes[pos] = (position, moves.isSimple);
                        queue.Enqueue(pos);
                    }
                }
            }

            path = Constants.EmptyField;
            return false;
        }

        private bool IsDestinationReached(FieldMask position)
        {
            return endMask.And(position).IsNotZero();
        }

        private (FieldMask[] masks, bool isSimple) GetPossibleMoves(FieldMask position)
        {
            return moveProvider.GetAvailableMovesWithType(field, position, enemy.Position);
        }
    }
}