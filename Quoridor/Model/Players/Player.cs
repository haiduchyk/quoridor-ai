namespace Quoridor.Model.Players
{
    using Moves;
    using Strategies;

    public class Player
    {
        public Player Enemy { get; private set; }

        public ref readonly FieldMask Position => ref position;

        public ref readonly FieldMask EndPosition => ref endPosition;

        public FieldMask Walls { get; private set; }

        public int AmountOfWalls { get; private set; }

        public string Name { get; }
        
        public FieldMask CurrentPath { get; set; }

        private readonly IMoveStrategy moveStrategy;
        private FieldMask endPosition;
        private FieldMask position;

        public Player()
        {
        }

        public Player(FieldMask position, int amountOfWalls, string name, IMoveStrategy moveStrategy,
            FieldMask endPosition)
        {
            this.position = position;
            AmountOfWalls = amountOfWalls;
            Name = name;
            this.moveStrategy = moveStrategy;
            this.endPosition = endPosition;
        }

        public void SetEnemy(Player enemy)
        {
            Enemy = enemy;
        }

        public bool HasReachedFinish()
        {
            return position.And(in endPosition).IsNotZero();
        }

        public bool IsEndPosition(in FieldMask checkPosition)
        {
            return checkPosition.And(in endPosition).IsNotZero();
        }

        public void ChangePosition(in FieldMask position)
        {
            this.position = position;
        }

        public bool ShouldWaitForMove()
        {
            return moveStrategy.IsManual;
        }

        public IMove FindMove(Field field)
        {
            return moveStrategy.FindMove(field, this);
        }

        public bool HasWalls()
        {
            return AmountOfWalls > 0;
        }

        public void UseWall(in FieldMask wall)
        {
            Walls = Walls.Or(in wall);
            AmountOfWalls--;
        }

        public void RestoreWall(in FieldMask wall)
        {
            Walls = Walls.And(wall.Not());
            AmountOfWalls++;
        }

        public void Update(Player player)
        {
            endPosition = player.EndPosition;
            position = player.Position;
            AmountOfWalls = player.AmountOfWalls;
        }
    }
}
