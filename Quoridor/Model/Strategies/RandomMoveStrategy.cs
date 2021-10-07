namespace Quoridor.Logic.Strategies
{
    using System;

    public class RandomMoveStrategy : IMoveStrategy
    {
        private readonly IMoveProvider moveProvider;
        private readonly Field field;

        private readonly Random random = new();

        public RandomMoveStrategy(IMoveProvider moveProvider, Field field)
        {
            this.moveProvider = moveProvider;
            this.field = field;
        }

        public FieldMask MakeMove(FieldMask playerMask)
        {
            var availableMoves = moveProvider.GetAvailableMoves(field, ref playerMask);
            var move = availableMoves[random.Next(0, availableMoves.Length)];
            return move;
        }
    }
}
