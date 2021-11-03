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

        public IMove FindMove(Field field, Player player)
        {
            if (!player.HasWalls())
            {
                return GetRandomPlayerMove(field, player);
            }
            return random.NextDouble() < 0.7
                ? GetRandomPlayerMove(field, player)
                : GetRandomWallMove(field, player);
        }

        private IMove GetRandomPlayerMove(Field field, Player player)
        {
            var availableMoves = moveProvider.GetAvailableMoves(field, in player.Position, in player.Enemy.Position);
            var move = availableMoves[random.Next(0, availableMoves.Length)];
            return new PlayerMove(player, move, field, search);
        }

        private IMove GetRandomWallMove(Field field, Player player)
        {
            var walls = wallProvider.GenerateWallMoves(field);
            var wall = walls[random.Next(0, walls.Count)];
            return new WallMove(field, player, search, wall);
        }
    }
}
