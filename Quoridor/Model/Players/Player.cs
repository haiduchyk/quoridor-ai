namespace Quoridor.Model.Players
{
    using Moves;
    using Strategies;

    public class Player
    {
        public Player Enemy { get; private set; }

        public FieldMask PositionMask => PlayerConstants.allPositions[Position];

        public ref readonly byte Position => ref position;

        public FieldMask Walls { get; private set; }

        public int AmountOfWalls { get; private set; }

        public int NumberOfMoves { get; private set; }

        public byte EndDownIndex => endDownIndex;

        public FieldMask CurrentPath { get; set; }

        private readonly IMoveStrategy moveStrategy;
        private byte endUpIndex;
        private byte endDownIndex;
        private byte position;

        public Player()
        {
        }

        public Player(byte position, int amountOfWalls, byte endUpIndex, byte endDownIndex, IMoveStrategy moveStrategy)
        {
            this.position = position;
            AmountOfWalls = amountOfWalls;
            this.endUpIndex = endUpIndex;
            this.endDownIndex = endDownIndex;
            this.moveStrategy = moveStrategy;
        }

        public void SetEnemy(Player enemy)
        {
            Enemy = enemy;
        }

        public bool HasReachedFinish()
        {
            // TODO index , rewrite using index
            return IsEndPosition(position);
        }

        public bool IsEndPosition(in byte checkPosition)
        {
            return checkPosition <= endUpIndex && checkPosition >= endDownIndex;
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
            endUpIndex = player.endUpIndex;
            endDownIndex = player.endDownIndex;
            position = player.Position;
            AmountOfWalls = player.AmountOfWalls;
            NumberOfMoves = player.NumberOfMoves;
        }
    }
}
