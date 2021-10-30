namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public class WallMove : IMove
    {
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
            field.PlaceWallAndUpdateMoves(in wall, player);
        }

        public void Apply(Field field, Player player)
        {
            this.field = field;
            this.player = player;
        }

        public FieldMask GetIdentifier()
        {
            return wall;
        }
    }
}
