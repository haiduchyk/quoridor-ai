namespace Quoridor.Model.Players
{
    using Moves;
    using Strategies;

    public class Player
    {
        public FieldMask Position { get; private set; }

        public int AmountOfWalls { get; private set; }

        private readonly IMoveStrategy moveStrategy;
        private readonly FieldMask endPosition;

        public Player(FieldMask position, FieldMask endPosition, int amountOfWalls, IMoveStrategy moveStrategy)
        {
            Position = position;
            AmountOfWalls = amountOfWalls;
            this.moveStrategy = moveStrategy;
            this.endPosition = endPosition;
        }

        public bool HasReachedEnd()
        {
            var position = Position;
            return !endPosition.And(ref position).IsZero();
        }

        public void ChangePosition(FieldMask position)
        {
            Position = position;
        }

        public bool ShouldWaitForMove()
        {
            return moveStrategy.IsManual;
        }

        public Move MakeMove(Field field, Player enemy)
        {
            return moveStrategy.MakeMove(field, this, enemy);
        }

        public bool HasWalls()
        {
            return AmountOfWalls > 0;
        }

        public void UseWall()
        {
            AmountOfWalls--;
        }

        public void RestoreWall()
        {
            AmountOfWalls++;
        }
    }
}
