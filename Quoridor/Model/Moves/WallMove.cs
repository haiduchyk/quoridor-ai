namespace Quoridor.Model.Moves
{
    using Model;
    using Players;
    using Strategies;

    public class WallMove : IMove
    {
        public FieldMask GetIdentifier => wall;

        private readonly ISearch search;
        private readonly FieldMask wall;
        private Field field;
        private Player player;

        public WallMove(Field field, Player player, ISearch search, FieldMask wall)
        {
            this.field = field;
            this.player = player;
            this.search = search;
            this.wall = wall;
        }

        public bool IsValid()
        {
            return player.HasWalls() && CheckPath();
        }

        private bool CheckPath()
        {
            field.PlaceWall(in wall);
            var hasPathForEnemy = search.HasPath(field, player.Enemy, in player.Enemy.Position, out _);
            var hasPathForPlayer = search.HasPath(field, player, in player.Position, out _);
            field.RemoveWall(in wall);
            return hasPathForPlayer && hasPathForEnemy;
        }

        public void Execute()
        {
            player.UseWall(wall);
            // TODO index
            // field.PlaceWallAndUpdateValidMoves(in wall, player);
        }

        public void Apply(Field field, Player player)
        {
            this.field = field;
            this.player = player;
        }

        protected bool Equals(WallMove other)
        {
            return wall.Equals(other.wall);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((WallMove) obj);
        }

        public override int GetHashCode()
        {
            return wall.GetHashCode();
        }

        public static bool operator ==(WallMove left, WallMove right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(WallMove left, WallMove right)
        {
            return !Equals(left, right);
        }
    }
}