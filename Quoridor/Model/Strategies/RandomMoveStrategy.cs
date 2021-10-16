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

        private readonly Random random = new();

        public RandomMoveStrategy(IMoveProvider moveProvider, IWallProvider wallProvider)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
        }

        public Move MakeMove(Field field, Player player, Player enemy)
        {
            return random.NextDouble() < 0.5 ? GetRandomPlayerMove(field, player, enemy) : GetRandomWallMove(field, player); 
        }

        private Move GetRandomPlayerMove(Field field, Player player, Player enemy)
        {
            var playerPosition = player.Position;
            var enemyPosition = enemy.Position;
            
            var availableMoves = moveProvider.GetAvailableMoves(field, ref playerPosition, enemyPosition);
            var move = availableMoves[random.Next(0, availableMoves.Length)];
            return new PlayerMove(field, player, move);
        }

        private Move GetRandomWallMove(Field field, Player player)
        {
            var walls = wallProvider.GenerateWallMoves(field);
            var wall = walls[random.Next(0, walls.Count)];
            return new WallMove(field, player, wall);
        }
    }
}
