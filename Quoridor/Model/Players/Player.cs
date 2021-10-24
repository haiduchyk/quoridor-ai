namespace Quoridor.Model.Players
{
    using Moves;
    using Strategies;

    public class Player
    {
        public FieldMask Position { get; private set; }

        public FieldMask Walls { get; private set; }

        public int AmountOfWalls { get; private set; }

        public string Name { get; }

        private readonly IMoveStrategy moveStrategy;
        private readonly FieldMask endPosition;

        public Player(FieldMask position, int amountOfWalls, string name, IMoveStrategy moveStrategy,
            FieldMask endPosition)
        {
            Position = position;
            AmountOfWalls = amountOfWalls;
            Name = name;
            this.moveStrategy = moveStrategy;
            this.endPosition = endPosition;
        }

        public bool HasReachedFinish()
        {
            return Position.And(in endPosition).IsNotZero();
        }

        public void ChangePosition(FieldMask position)
        {
            Position = position;
        }

        public bool ShouldWaitForMove()
        {
            return moveStrategy.IsManual;
        }

        public IMove FindMove(Field field, Player enemy)
        {
            return moveStrategy.FindMove(field, this, enemy);
        }

        public bool HasWalls()
        {
            return AmountOfWalls > 0;
        }

        public void UseWall(FieldMask wall)
        {
            Walls = Walls.Or(in wall);
            AmountOfWalls--;
        }

        public void RestoreWall(FieldMask wall)
        {
            Walls = Walls.And(wall.Not());
            AmountOfWalls++;
        }

        public Player Copy()
        {
            return new Player(Position, AmountOfWalls, Name, moveStrategy, endPosition);
        }

        public void Update(Player player)
        {
            Position = player.Position;
            AmountOfWalls = player.AmountOfWalls;
        }
    }
}
