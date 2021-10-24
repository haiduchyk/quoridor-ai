namespace Quoridor.Model.Strategies
{
    using System;
    using System.Linq;
    using Moves;
    using Players;

    public class HeuristicStrategy : IMoveStrategy
    {
        public bool IsManual => false;

        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;

        private readonly Random random = new();

        public HeuristicStrategy(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
        }

        public IMove FindMove(Field field, Player player, Player enemy, FieldMask path)
        {
            if (!player.HasWalls())
            {
                return GetMoveOnPath(field, player, enemy, path);
            }
            return random.NextDouble() < 0.7
                ? GetMoveOnPath(field, player, enemy, path)
                : GetRandomWallMove(field, player, enemy);
        }

        private IMove GetMoveOnPath(Field field, Player player, Player enemy, FieldMask path)
        {
            var playerPosition = player.Position;
            var enemyPosition = enemy.Position;
            var availableMoves = moveProvider.GetAvailableMoves(field, in playerPosition, in enemyPosition);
            var move = availableMoves.First(m => m.And(in path).IsNotZero());
            return new PlayerMove(player, move);
        }

        public IMove FindMove(Field field, Player player, Player enemy)
        {
            return new DefaultMove();
        }

        private IMove GetRandomWallMove(Field field, Player player, Player enemy)
        {
            var walls = wallProvider.GenerateWallMoves(field, player, enemy);
            var wall = walls[random.Next(0, walls.Length)];
            return new WallMove(field, player, enemy, search, wall);
        }
    }
}
