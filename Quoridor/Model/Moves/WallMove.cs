namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public class WallMove : IMove
    {
        public ref readonly byte Id => ref wallIndex;

        private readonly ISearch search;
        private readonly byte wallIndex;
        private Field field;
        private Player player;

        public WallMove(Field field, Player player, ISearch search, byte wallIndex)
        {
            this.field = field;
            this.player = player;
            this.search = search;
            this.wallIndex = wallIndex;
        }

        public void Execute()
        {
            player.UseWall(Id);
            field.PlaceWallAndUpdateValidMoves(in wallIndex, player);
            search.UpdatePathForPlayers(field, player);
        }

        public void Apply(Field field, Player player)
        {
            this.field = field;
            this.player = player;
        }

        protected bool Equals(WallMove other)
        {
            return wallIndex.Equals(other.wallIndex);
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

            return Equals((WallMove)obj);
        }

        public override int GetHashCode()
        {
            return wallIndex.GetHashCode();
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
