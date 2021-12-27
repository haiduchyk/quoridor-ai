namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Controller.Moves;
    using Moves;
    using Players;

    public class MonteCarloStrategy : IMoveStrategy
    {
        private const long ComputeTime = 4990;
        private const double C = 1.4142135;
        private const int MaxThreadsAmount = 4;
        private const double ThresholdForBestNode = 0.01f;


        public bool IsManual => false;

        private readonly IMoveConverter moveConverter;
        private readonly MonteCarloMoveProvider monteCarloMoveProvider;
        private readonly Random random;

        private MonteNode root;

        private Player[] montePlayers;
        private Player[] monteEnemies;
        private Field[] monteFields;
        private HeuristicStrategy[] strategies;
        private MonteThread[] threads;
        private Task<int>[] tasks = new Task<int>[MaxThreadsAmount];

        private Field monteField => monteFields[0];
        private Player montePlayer => montePlayers[0];
        private readonly Player monteEnemy;

        public MonteCarloStrategy(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search,
            IMoveConverter moveConverter)
        {
            this.moveConverter = moveConverter;
            random = new Random();

            montePlayers = new Player[MaxThreadsAmount];
            monteEnemies = new Player[MaxThreadsAmount];
            monteFields = new Field[MaxThreadsAmount];
            strategies = new HeuristicStrategy[MaxThreadsAmount];
            threads = new MonteThread[MaxThreadsAmount];

            for (var i = 0; i < MaxThreadsAmount; i++)
            {
                var newSearch = search.Copy();
                var enemy = new Player();
                var player = new Player();

                montePlayers[i] = player;
                monteEnemies[i] = enemy;

                player.SetEnemy(enemy);
                enemy.SetEnemy(player);

                monteFields[i] = new Field(newSearch);
                strategies[i] = new HeuristicStrategy(moveProvider, wallProvider, newSearch);
                threads[i] = new MonteThread();
            }

            monteCarloMoveProvider =
                new MonteCarloMoveProvider(moveProvider, wallProvider, search, monteField, montePlayer);
        }

        public IMove FindMove(Field field, Player player, IMove lastMove, bool shouldPunish = false)
        {
            UpdateFields(field, player);
            SetRoot(lastMove);

            var startTime = GetCurrentTime();
            var count = 0;

            while (HasTime(startTime))
            {
                UpdateFields(field, player);
                var node = Select(root);
                var result = SimulateMultiple(node);
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
            for (var i = 0; i < MaxThreadsAmount; i++)
            {
                montePlayers[i].Update(player);
                monteEnemies[i].Update(player.Enemy);
                monteFields[i].Update(field);
            }
        }


        private MonteNode FindBest(MonteNode node)
        {
            var bestShift = FindBest(node.children.Where(n => n.move.IsMove));
            var bestNode = FindBest(node.children);
            return bestShift == null || bestNode.WinRate - bestShift.WinRate > ThresholdForBestNode ? bestNode : bestShift;
        }

        private MonteNode FindBest(IEnumerable<MonteNode> nodes)
        {
            var max = float.NegativeInfinity;
            MonteNode best = null;

            foreach (var child in nodes)
            {
                var estimation = GetEstimate(child);
                if (estimation > max)
                {
                    max = estimation;
                    best = child;
                }
            }

            return best;
        }

        private float GetEstimate(MonteNode node)
        {
            return node.WinRate * 80 + (float) node.games / root.games * 20;
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

            var expand = (double) node.wins / node.games;
            expand = node.IsPlayerMove ? expand : 1 - expand;
            var explore = C * Math.Sqrt(Math.Log10(node.parent.games) / node.games);
            return expand + explore;
        }

        private MonteNode PickUnvisited(MonteNode node)
        {
            var unvisited = node.children.Where(n => !n.IsVisited).ToArray();
            var child = GetRandomWithHeuristic(unvisited);
            child.move.Execute();
            child.SetChild(FindChildren(child));
            return child;
        }

        private MonteNode GetRandomWithHeuristic(MonteNode[] nodes)
        {
            var child = nodes.FirstOrDefault(n => n.move.IsMove);
            return child ?? nodes[random.Next(0, nodes.Length)];
        }

        private int SimulateMultiple(MonteNode node)
        {
            for (var i = 0; i < MaxThreadsAmount; i++)
            {
                var thread = threads[i];
                thread.montePlayer = montePlayers[i];
                thread.monteEnemy = monteEnemies[i];
                thread.monteField = monteFields[i];
                thread.strategy = strategies[i];
                thread.node = node;

                var task = Task.Factory.StartNew(thread.Simulate);
                tasks[i] = task;
            }

            var result = tasks.Sum(task => task.GetAwaiter().GetResult());

            return result;
        }

        private void Backpropagate(MonteNode node, int result)
        {
            while (node.parent != null)
            {
                node.Update(result, MaxThreadsAmount);
                node = node.parent;
            }

            node.Update(result, MaxThreadsAmount);
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
            var name = montePlayers[0].EndDownIndex == PlayerConstants.EndBlueDownIndexIncluding ? "Blue" : "Red";
            Console.WriteLine($"{name}");
            Console.WriteLine($"Count => {count}");
            Console.WriteLine($"Time => {GetTime(startTime)}");
            Console.WriteLine($"Depth => {GetDepth(root)}");
            var (branching, nodes) = GetNodeStatistic(root);
            Console.WriteLine($"Average branching => {(float) branching / nodes}");
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
            if (node.children == null || node.level > 1)
            {
                return;
            }

            var offset = "".PadLeft(node.level * 2, '-');
            Console.WriteLine(
                $"{offset}{moveConverter.GetCode(monteFields[0], montePlayers[0], node.move)} -> {node.WinRate}");
            foreach (var child in node.children.OrderByDescending(n => n.WinRate))
            {
                PrintTree(child);
            }
        }
    }
}