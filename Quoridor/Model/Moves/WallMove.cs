namespace Quoridor.Model.Moves
{
    using System.Linq;
    using Model;
    using Players;
    using Strategies;

    public class WallMove : IMove
    {
        public ref readonly byte Id => ref wallIndex;

        private readonly IWallProvider wallProvider;
        private readonly ISearch search;
        private readonly byte wallIndex;
        public Field field;
        private Player player;

        public WallMove(Field field, Player player, ISearch search, IWallProvider wallProvider, byte wallIndex)
        {
            this.field = field;
            this.player = player;
            this.search = search;
            this.wallProvider = wallProvider;
            this.wallIndex = wallIndex;
        }

        public bool IsValid()
        {
            var nearWalls = WallConstants.NearWallsToCheck[wallIndex].Intersect(field.PlacedWalls).Count();
            nearWalls += WallConstants.NearEdgeWalls.Contains(wallIndex) ? 1 : 0;
            if (nearWalls < 2)
            {
                return true;
            }
            field.PlaceWall(wallIndex);
            var isValid = search.HasPath(field, player, player.Position) &&
                          search.HasPath(field, player.Enemy, player.Enemy.Position);
            field.RemoveWall(wallIndex);
            return isValid;
        }

        public void Execute()
        {
            player.UseWall(wallIndex);
            field.PlaceWallAndUpdatePossibleWalls(in wallIndex, player);
            search.UpdatePathForPlayers(field, player);
            if (wallProvider.HasCachedWalls(field, player, out var walls))
            {
                field.SetValidWalls(walls);
            }
            else
            {
                field.UpdateValidWalls(in wallIndex, player);
                wallProvider.SetCachedWalls(field, player, field.PossibleWalls);
            }
        }

        public void ExecuteForSimulation()
        {
            player.UseWall(wallIndex);
            field.PlaceWallAndUpdatePossibleWalls(in wallIndex, player);
        }

        public void Apply(Field field, Player player)
        {
            this.field = field;
            this.player = player;
        }

        public void Log()
        {
            WallConstants.AllWalls[Id].Log();
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
