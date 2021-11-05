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

        private readonly Random random = new(1);

        public HeuristicStrategy(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
        }

        public IMove FindMove(Field field, Player player, IMove lastMove)
        {
            return random.NextDouble() < 0.7
                ? GetMoveOnPath(field, player)
                : GetRandomWallMove(field, player);
        }

        private IMove GetMoveOnPath(Field field, Player player)
        {
            // TODO make list of shortest path of bytes in Player 
            // TODO dont take random, take bigger row
            var availableMoves = moveProvider.GetAvailableMoves(field, in player.Position, in player.Enemy.Position);
            // TODO Fix this check, don't need it
            var move = availableMoves.Length == 0
                ? player.Position
                : availableMoves[random.Next(0, availableMoves.Length)];
            return new PlayerMove(player, move, field, search, wallProvider);
        }

        private IMove GetRandomWallMove(Field field, Player player)
        {
            if (!player.HasWalls())
            {
                return GetBackMoveOnPath(field, player);
                // TODO: test which faster
                return new PlayerMove(player, player.Position, field, search, wallProvider);
            }

            var walls = wallProvider.GenerateWallMoves(field);
            var wall = walls[random.Next(0, walls.Count)];
            return new WallMove(field, player, search, wallProvider, wall);
        }

        private IMove GetBackMoveOnPath(Field field, Player player)
        {
            var availableMoves = moveProvider.GetAvailableMoves(field, in player.Position, in player.Enemy.Position);
            byte move;
            // TODO Fix this check(== 0), don't need it
            if (availableMoves.Length == 0)
            {
                move = player.Position;
            }
            else if (player.EndDownIndex == PlayerConstants.EndRedDownIndexIncluding)
            {
                move = availableMoves.First();
            }
            else
            {
                move = availableMoves.Last();
            }
            return new PlayerMove(player, move, field, search, wallProvider);
        }
    }
}