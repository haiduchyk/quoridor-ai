namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public class WallMove : Move
    {
        public WallMove(Field field, Player player, FieldMask fieldMask) : base(field, player, fieldMask)
        {
        }

        public override bool IsValid()
        {
            return player.HasWalls();
        }

        public override void Execute()
        {
            player.UseWall();
            field.PlaceWall(in fieldMask);
        }

        public override void Undo()
        {
            player.RestoreWall();
            field.RemoveWall(in fieldMask);
        }
    }
}
