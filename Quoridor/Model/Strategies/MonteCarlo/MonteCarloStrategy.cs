namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moves;
    using Players;

    public class MonteCarloStrategy : IMoveStrategy
    {
        private const long ComputeTime = 2000;
        private const double C = 1.4142135;

        public bool IsManual => false;

        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;
        private readonly HeuristicStrategy strategy;
        private readonly Random random;

        private readonly Field fieldCopy;
        private readonly Player playerCopy;
        private readonly Player enemyCopy;

        public MonteCarloStrategy(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search)
        {
            fieldCopy = new Field();
            playerCopy = new Player();
            enemyCopy = new Player();
            playerCopy.SetEnemy(enemyCopy);
            enemyCopy.SetEnemy(playerCopy);
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
            strategy = new HeuristicStrategy(moveProvider, wallProvider, search);
            random = new Random();
        }

        public IMove FindMove(Field field, Player player)
        {
            fieldCopy.Update(field);
            playerCopy.Update(player);
            enemyCopy.Update(player.Enemy);
            var startTime = GetCurrentTime();
            var root = new MonteNode();
            root.SetChild(FindChildren(root));
            var count = 0;

            while (HasTime(startTime))
            {
                fieldCopy.Update(field);
                playerCopy.Update(player);
                enemyCopy.Update(player.Enemy);
                var node = Select(root);
                var result = Simulate(node);
                Backpropagate(node, result);
                count++;
            }
            var move = FindBest(root);
            move.Apply(field, player);
            Console.WriteLine($"Count => {count}");
            Console.WriteLine($"Time => {GetTime(startTime)}");
            Console.WriteLine($"Depth => {GetDepth(root)}");
            var (branching, nodes) = GetNodeStatistic(root);
            Console.WriteLine($"Average branching => {(float)branching / nodes}");
            return move;
        }

        private IMove FindBest(MonteNode node)
        {
            return node.children.OrderByDescending(n => n.WinRate).First().move;
        }

        private MonteNode[] FindChildren(MonteNode node)
        {
            var turnPlayer = node.IsPlayerMove ? playerCopy : enemyCopy;
            var turnEnemy = node.IsPlayerMove ? enemyCopy : playerCopy;
            var shifts = moveProvider.GetAvailableMoves(fieldCopy, in turnPlayer.Position, in turnEnemy.Position);
            var moves = shifts.Select<FieldMask, IMove>(m => new PlayerMove(turnPlayer, m)).ToList();
            if (turnPlayer.AmountOfWalls > 0)
            {
                moves.AddRange(GetWallMoves(turnPlayer));
            }
            return moves.Select(m => new MonteNode(node, m, node.level + 1)).ToArray();
        }

        private IEnumerable<IMove> GetWallMoves(Player player)
        {
            var walls = wallProvider.GenerateWallMoves(fieldCopy, player);
            return walls.Select<FieldMask, IMove>(w => new WallMove(fieldCopy, player, search, w));
        }

        private MonteNode Select(MonteNode node)
        {
            while (node.IsFullyExpanded)
            {
                node = FindBestUct(node);
                node.move.Execute();
            }
            var chosenNode = PickUnvisited(node);
            chosenNode.move.Execute();
            return chosenNode;
        }

        private MonteNode FindBestUct(MonteNode node)
        {
            var bestUtc = double.NegativeInfinity;
            var bestNodes = new List<MonteNode>(node.children.Length);
            foreach (var child in node.children)
            {
                var utc = CalculateUct(child);
                if (utc > bestUtc)
                {
                    bestUtc = utc;
                    bestNodes.Clear();
                }
                if (utc == bestUtc)
                {
                    bestNodes.Add(child);
                }
            }
            return bestNodes[random.Next(0, bestNodes.Count)];
        }

        private double CalculateUct(MonteNode node)
        {
            if (node.games == 0)
            {
                return double.PositiveInfinity;
            }
            var expand = (double)node.wins / node.games;
            expand = node.IsPlayerMove ? expand : 1 - expand;
            var explore = C * Math.Sqrt(Math.Log10(node.parent.games) / node.games);
            return expand + explore;
        }

        private MonteNode PickUnvisited(MonteNode node)
        {
            var unvisited = node.children.Where(n => !n.IsVisited).ToArray();
            var child = unvisited[random.Next(0, unvisited.Length)];
            child.SetChild(FindChildren(child));
            return child;
        }

        private int Simulate(MonteNode node)
        {
            var firstPlayer = node.IsPlayerMove ? playerCopy : enemyCopy;
            var secondPlayer = node.IsPlayerMove ? enemyCopy : playerCopy;
            var moveCount = 0;
            while (!firstPlayer.HasReachedFinish() && !secondPlayer.HasReachedFinish() && moveCount < 100)
            {
                var player = moveCount % 2 == 0 ? firstPlayer : secondPlayer;
                var move = strategy.FindMove(fieldCopy, player);
                if (move.IsValid())
                {
                    move.Execute();
                    moveCount++;
                }
            }
            return playerCopy.HasReachedFinish() ? 1 : 0;
        }

        private void Backpropagate(MonteNode node, int result)
        {
            while (node.parent != null)
            {
                node.Update(result);
                node = node.parent;
            }
            node.Update(result);
        }

        private long GetCurrentTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private bool HasTime(long startTime)
        {
            return GetCurrentTime() - startTime < ComputeTime;
        }

        private float GetTime(long startTime)
        {
            var milliseconds = GetCurrentTime() - startTime;
            return milliseconds / 1000f;
        }

        private int GetDepth(MonteNode node)
        {
            return node.children == null ? node.level : node.children.Select(GetDepth).Max();
        }

        private (int brancing, int nodes) GetNodeStatistic(MonteNode node)
        {
            if (node.children != null)
            {
                var branching = node.children.Length;
                var nodes = 1;
                foreach (var child in node.children)
                {
                    var (childBranching, childNodes) = GetNodeStatistic(child);
                    branching += childBranching;
                    nodes += childNodes;
                }
                return (branching, nodes);
            }
            return (0, 0);
        }
    }
}
