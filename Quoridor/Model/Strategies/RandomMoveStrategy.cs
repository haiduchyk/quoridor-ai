namespace Quoridor.Model.Strategies
{
    using System;
    using Moves;
    using Players;

    public class RandomMoveStrategy : IMoveStrategy
    {
        public bool IsManual => false;

        private readonly IMoveProvider moveProvider;

        private readonly Random random = new();

        public RandomMoveStrategy(IMoveProvider moveProvider)
        {
            this.moveProvider = moveProvider;
        }

        public Move MakeMove(Field field, Player player)
        {
            var playerPosition = player.Position;
            var availableMoves = moveProvider.GetAvailableMoves(field, ref playerPosition);
            var move = availableMoves[random.Next(0, availableMoves.Length)];
            return new PlayerMove(field, player, move);
        }
    }
}
