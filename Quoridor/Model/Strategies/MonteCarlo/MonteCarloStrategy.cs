namespace Quoridor.Model.Strategies
{
    using System;
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
            var moves = moveProvider.GetAvailableMoves(fieldCopy, in turnPlayer.Position, in turnEnemy.Position);
            var playerMoves = moves.Select<FieldMask, IMove>(m => new PlayerMove(turnPlayer, m));
            var walls = wallProvider.GenerateWallMoves(fieldCopy, playerCopy);
            var wallMoves =
                walls.Select<FieldMask, IMove>(w => new WallMove(fieldCopy, turnPlayer, search, w));
            var possibleMoves = playerMoves.Concat(wallMoves);
            return possibleMoves.Select(m => new MonteNode(node, m, node.level + 1)).ToArray();
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
            return node.children.OrderByDescending(CalculateUct).First();
        }

        private double CalculateUct(MonteNode node)
        {
            var expand = (double)node.wins / node.games;
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
        }

        private long GetCurrentTime()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        private bool HasTime(long startTime)
        {
            return GetCurrentTime() - startTime < ComputeTime;
        }
    }
}
