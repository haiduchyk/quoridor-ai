namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Controller.Moves;
    using Moves;
    using Players;

    public class MonteCarloStrategy : IMoveStrategy
    {
        private static readonly double[] HeuristicCoef =
            { 0.9, 0.87, 0.84, 0.81, 0.78, 0.75, 0.72, 0.69, 0.66, 0.63, 0.6 };

        private const long ComputeTime = 2000;
        private const double C = 1.4142135;

        public bool IsManual => false;

        private readonly IMoveConverter moveConverter;
        private readonly MonteCarloMoveProvider monteCarloMoveProvider;
        private readonly HeuristicStrategy strategy;
        private readonly Random random;

        private readonly Field monteField;
        private readonly Player montePlayer;
        private readonly Player monteEnemy;
        private MonteNode root;

        public MonteCarloStrategy(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search,
            IMoveConverter moveConverter)
        {
            this.moveConverter = moveConverter;
            monteField = new Field(search);
            montePlayer = new Player();
            monteEnemy = new Player();
            montePlayer.SetEnemy(monteEnemy);
            monteEnemy.SetEnemy(montePlayer);
            monteCarloMoveProvider =
                new MonteCarloMoveProvider(moveProvider, wallProvider, search, monteField, montePlayer);
            strategy = new HeuristicStrategy(moveProvider, wallProvider, search);
            random = new Random();
        }

        public IMove FindMove(Field field, Player player, IMove lastMove)
        {
            UpdateFields(field, player);
            SetRoot(lastMove);

            var startTime = GetCurrentTime();
            var count = 0;

            while (HasTime(startTime))
            {
                UpdateFields(field, player);
                var node = Select(root);
                var result = Simulate(node);
                Backpropagate(node, result);
                count++;
            }

            var bestNode = FindBest(root);
            PrintStatistic(count, startTime, bestNode);
            PrintTree();

            var move = bestNode.move;
            move.Apply(field, player);
            root = bestNode;

            return move;
        }

        private void SetRoot(IMove lastMove)
        {
            if (root == null)
            {
                CreateNewRoot();
            }
            else
            {
                FindNewRootFromChildren(lastMove);
            }
        }

        private void CreateNewRoot()
        {
            root = new MonteNode();
            root.SetChild(FindChildren(root));
        }

        private void FindNewRootFromChildren(IMove lastMove)
        {
            var nextRoot = root.GetNextRoot(lastMove);

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
            return node.children.OrderByDescending(GetEstimate).First();
        }

        private double GetEstimate(MonteNode node)
        {
            return node.WinRate * 80 + (double)node.games / root.games * 20;
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
            expand = !node.IsPlayerMove ? expand : 1 - expand;
            var explore = C * Math.Sqrt(Math.Log10(node.parent.games) / node.games);
            return expand + explore;
        }

        private MonteNode PickUnvisited(MonteNode node)
        {
            var player = node.IsPlayerMove ? monteEnemy : montePlayer;
            var unvisited = node.children.Where(n => !n.IsVisited).ToArray();
            var child = GetRandomWithHeuristic(unvisited, player);
            child.move.Execute();
            child.SetChild(FindChildren(child));
            return child;
        }

        private MonteNode GetRandomWithHeuristic(MonteNode[] nodes, Player player)
        {
            var child = random.NextDouble() < HeuristicCoef[10 - player.AmountOfWalls]
                ? nodes.FirstOrDefault(n => n.move.IsMove)
                : nodes.FirstOrDefault(n => !n.move.IsMove);
            return child ?? nodes[random.Next(0, nodes.Length)];
        }

        private int Simulate(MonteNode node)
        {
            var firstPlayer = node.IsPlayerMove ? monteEnemy : montePlayer;
            var secondPlayer = node.IsPlayerMove ? montePlayer : monteEnemy;
            var moveCount = 0;
            while (!firstPlayer.HasReachedFinish() && !secondPlayer.HasReachedFinish() && moveCount < 400)
            {
                var player = moveCount % 2 == 0 ? firstPlayer : secondPlayer;
                var move = strategy.FindMove(monteField, player, null);
                if (move.IsValid())
                {
                    move.ExecuteForSimulation();
                    moveCount++;
                }
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
            // PrintTree(root);
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

        private void PrintTree()
        {
#if DEBUG
            PrintTree(root);
#endif
        }

        private void PrintTree(MonteNode node)
        {
            if (node.children == null || node.level > 0)
            {
                return;
            }

            var offset = "".PadLeft(node.level * 2, '-');
            Console.WriteLine($"{offset}{moveConverter.GetCode(monteField, montePlayer, node.move)} -> {node.WinRate}");
            foreach (var child in node.children.OrderByDescending(n => n.WinRate))
            {
                PrintTree(child);
            }
        }
    }
}