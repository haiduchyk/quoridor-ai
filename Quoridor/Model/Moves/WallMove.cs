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
        private Player enemy;

        public WallMove(Field field, Player player, Player enemy, ISearch search, FieldMask wall)
        {
            this.field = field;
            this.player = player;
            this.enemy = enemy;
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
            Execute();
            var hasPathForEnemy = search.HasPath(field, enemy, enemy.Position, out _);
            var hasPathForPlayer = search.HasPath(field, player, player.Position, out _);
            field.RemoveWall(in wall);
            return hasPathForPlayer && hasPathForEnemy;
        }

        public void Execute()
        {
            player.UseWall(wall);
            field.PlaceWallAndUpdateMoves(in wall);
        }

        public void Undo()
        {
            player.RestoreWall(wall);
            field.RemoveWall(in wall);
        }

        public void Apply(Field field, Player player, Player enemy)
        {
            this.field = field;
            this.player = player;
            this.enemy = enemy;
        }
    }
}
