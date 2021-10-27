namespace Quoridor.Model.Strategies
{
    using System;
    using Moves;
    using Players;

    public class RandomMoveStrategy : IMoveStrategy
    {
        public bool IsManual => false;

        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;

        private readonly Random random = new();

        public RandomMoveStrategy(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
        }

        public IMove FindMove(Field field, Player player, Player enemy)
        {
            if (!player.HasWalls())
            {
                return GetRandomPlayerMove(field, player, enemy);
            }
            return random.NextDouble() < 0.7
                ? GetRandomPlayerMove(field, player, enemy)
                : GetRandomWallMove(field, player, enemy);
        }

        private IMove GetRandomPlayerMove(Field field, Player player, Player enemy)
        {
            var playerPosition = player.Position;
            var enemyPosition = enemy.Position;
            var availableMoves = moveProvider.GetAvailableMoves(field, playerPosition, enemyPosition);
            var move = availableMoves[random.Next(0, availableMoves.Length)];
            return new PlayerMove(player, move);
        }

        private IMove GetRandomWallMove(Field field, Player player, Player enemy)
        {
            var walls = wallProvider.GenerateWallMoves(field);
            var wall = walls[random.Next(0, walls.Count)];
            return new WallMove(field, player, enemy, search, wall);
        }
    }
}
