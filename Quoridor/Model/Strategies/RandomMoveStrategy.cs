namespace Quoridor.Model.Strategies
{
    using System;

    public class RandomMoveStrategy : IMoveStrategy
    {
        public bool IsManual => false;

        private readonly IMoveProvider moveProvider;
        private readonly Random random = new();

        public RandomMoveStrategy(IMoveProvider moveProvider)
        {
            this.moveProvider = moveProvider;
        }

        public FieldMask MakeMove(Field field, FieldMask playerMask)
        {
            var availableMoves = moveProvider.GetAvailableMoves(field, ref playerMask);
            var move = availableMoves[random.Next(0, availableMoves.Length)];
            return move;
        }
    }
}
