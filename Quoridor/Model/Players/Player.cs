namespace Quoridor.Model.Players
{
    using Moves;
    using Strategies;

    public class Player
    {
        public Player Enemy { get; private set; }

        public FieldMask PositionMask => PlayerConstants.allPositions[Position];
        
        public ref readonly byte Position => ref position;

        public ref readonly FieldMask EndPosition => ref endPosition;

        public FieldMask Walls { get; private set; }

        public int AmountOfWalls { get; private set; }

        public int NumberOfMoves { get; private set; }

        public FieldMask CurrentPath { get; set; }

        private readonly IMoveStrategy moveStrategy;
        private FieldMask endPosition;
        private byte position;

        public Player()
        {
        }

        public Player(byte position, int amountOfWalls, FieldMask endPosition, IMoveStrategy moveStrategy)
        {
            this.position = position;
            AmountOfWalls = amountOfWalls;
            this.moveStrategy = moveStrategy;
            this.endPosition = endPosition;
        }

        public void SetEnemy(Player enemy)
        {
            Enemy = enemy;
        }

        public bool HasReachedFinish()
        {
            // TODO index , rewrite using index
            return PositionMask.And(in endPosition).IsNotZero();
        }

        public bool IsEndPosition(in FieldMask checkPosition)
        {
            return checkPosition.And(in endPosition).IsNotZero();
        }

        public void ChangePosition(in byte position)
        {
            this.position = position;
            NumberOfMoves++;
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

        public void UseWall(in byte wall)
        {
            Walls = Walls.Or(in WallConstants.AllWalls[wall]);
            AmountOfWalls--;
            NumberOfMoves++;
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
            NumberOfMoves = player.NumberOfMoves;
        }
    }
}
