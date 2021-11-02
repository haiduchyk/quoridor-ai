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

        private readonly MonteCarloMoveProvider monteCarloMoveProvider;
        private readonly HeuristicStrategy strategy;
        private readonly Random random;

        private readonly Field monteField;
        private readonly Player montePlayer;
        private readonly Player monteEnemy;
        private MonteNode root;

        public MonteCarloStrategy(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search)
        {
            monteField = new Field(search);
            montePlayer = new Player();
            monteEnemy = new Player();
            montePlayer.SetEnemy(monteEnemy);
            monteEnemy.SetEnemy(montePlayer);
            monteCarloMoveProvider =
                new MonteCarloMoveProvider(moveProvider, wallProvider, search, monteField, montePlayer);
            strategy = new HeuristicStrategy(moveProvider, wallProvider, search);
            random = new Random(1);
        }

        public IMove FindMove(Field field, Player player)
        {
            UpdateFields(field, player);
            SetRoot();

            var startTime = GetCurrentTime();
            var count = 0;

            while (count < 100)
            // while (HasTime(startTime))
            {
                UpdateFields(field, player);
                var node = Select(root);
                var result = Simulate(node);
                Backpropagate(node, result);
                count++;
            }

            var bestNode = FindBest(root);
            PrintStatistic(count, startTime, bestNode);

            var move = bestNode.move;
            move.Apply(field, player);
            root = bestNode;

            return move;
        }

        private void SetRoot()
        {
            if (root == null)
            {
                CreateNewRoot();
            }
            else
            {
                FindNewRootFromChildren();
            }
        }

        private void CreateNewRoot()
        {
            root = new MonteNode();
            root.SetChild(FindChildren(root));
        }

        private void FindNewRootFromChildren()
        {
            var nextRoot = root.GetNextRoot();

            if (nextRoot == null)
            {
                CreateNewRoot();
            }
            else
            {
                root = nextRoot;
                if (root.children == null)
                {
                    root.SetChild(FindChildren(root));
                }
            }
        }

        private void UpdateFields(Field field, Player player)
        {
            monteField.Update(field);
            montePlayer.Update(player);
            monteEnemy.Update(player.Enemy);
        }

        private MonteNode FindBest(MonteNode node)
        {
            return node.children.OrderByDescending(n => n.WinRate).First();
        }

        private MonteNode[] FindChildren(MonteNode node)
        {
            return monteCarloMoveProvider.FindMoves(node)
                .Select(m => new MonteNode(node, m, node.level + 1))
                .ToArray();
        }

        private MonteNode Select(MonteNode node)
        {
            while (node.IsFullyExpanded && !node.IsTerminal)
            {
                node = FindBestUct(node);
                node.move.Execute();
            }

            if (node.IsTerminal)
            {
                return node;
            }
            var chosenNode = PickUnvisited(node);
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

            child.move.Execute();
            child.SetChild(FindChildren(child));
            return child;
        }

        private int Simulate(MonteNode node)
        {
            var firstPlayer = node.IsPlayerMove ? montePlayer : monteEnemy;
            var secondPlayer = node.IsPlayerMove ? monteEnemy : montePlayer;
            var moveCount = 0;
            while (!firstPlayer.HasReachedFinish() && !secondPlayer.HasReachedFinish() && moveCount < 100)
            {
                var player = moveCount % 2 == 0 ? firstPlayer : secondPlayer;
                var move = strategy.FindMove(monteField, player);
                // if (move.IsValid())
                // {
                    move.Execute();
                    moveCount++;
                // }
            }

            return montePlayer.HasReachedFinish() ? 1 : 0;
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

        private void PrintStatistic(int count, long startTime, MonteNode bestNode)
        {
#if DEBUG
            var name = montePlayer.EndDownIndex == PlayerConstants.EndBlueDownIndexIncluding ? "Blue" : "Red";
            Console.WriteLine($"{name}");
            Console.WriteLine($"Count => {count}");
            Console.WriteLine($"Time => {GetTime(startTime)}");
            Console.WriteLine($"Depth => {GetDepth(root)}");
            var (branching, nodes) = GetNodeStatistic(root);
            Console.WriteLine($"Average branching => {(float)branching / nodes}");
            Console.WriteLine($"Win rate in root => {root.WinRate:F4}");
            Console.WriteLine($"Win rate in best => {bestNode.WinRate:F4}");
#endif
        }

        private float GetTime(long startTime)
        {
            var milliseconds = GetCurrentTime() - startTime;
            return milliseconds / 1000f;
        }

        private int GetDepth(MonteNode node)
        {
            return node.children == null || node.children.Length == 0
                ? node.level
                : node.children.Select(GetDepth).Max();
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
