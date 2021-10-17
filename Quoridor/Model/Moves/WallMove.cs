namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public class WallMove : IMove
    {
        private readonly Field field;
        private readonly Player player;
        private readonly Player enemy;
        private readonly ISearch search;
        private readonly FieldMask wall;

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
            Execute();
            var hasPathForEnemy = search.HasPath(enemy, enemy.Position, out _);
            var hasPathForPlayer = search.HasPath(player, player.Position, out _);
            Undo();
            return hasPathForPlayer && hasPathForEnemy;
        }

        public void Execute()
        {
            player.UseWall(wall);
            field.PlaceWall(in wall);
        }

        public void Undo()
        {
            player.RestoreWall(wall);
            field.RemoveWall(in wall);
        }
    }
}
