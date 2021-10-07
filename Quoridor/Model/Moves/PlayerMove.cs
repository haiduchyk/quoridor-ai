namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public class PlayerMove : Move
    {
        private readonly FieldMask previousPosition;

        public PlayerMove(Field field, Player player, FieldMask fieldMask) : base(field, player, fieldMask)
        {
            previousPosition = player.Position;
        }

        public override bool IsValid()
        {
            return true;
        }

        public override void Execute()
        {
            player.ChangePosition(fieldMask);
        }

        public override void Undo()
        {
            player.ChangePosition(previousPosition);
        }
    }
}
