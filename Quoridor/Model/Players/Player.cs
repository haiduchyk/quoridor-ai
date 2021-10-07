namespace Quoridor.Model.Players
{
    using Strategies;

    public class Player
    {
        public FieldMask Position { get; private set; }

        public int AmountOfWalls { get; private set; }

        private IMoveStrategy moveStrategy;

        public Player(FieldMask position, int amountOfWalls, IMoveStrategy moveStrategy)
        {
            Position = position;
            AmountOfWalls = amountOfWalls;
            this.moveStrategy = moveStrategy;
        }

        public void ChangePosition(FieldMask position)
        {
            Position = position;
        }

        public bool ShouldWaitForMove()
        {
            return moveStrategy.IsManual;
        }

        public FieldMask MakeMove(Field field)
        {
            return moveStrategy.MakeMove(field, Position);
        }

        public bool HasWalls()
        {
            return AmountOfWalls > 0;
        }

        public void UseWall()
        {
            AmountOfWalls--;
        }
    }
}
