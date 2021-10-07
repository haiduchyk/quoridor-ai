namespace Quoridor.Model.Moves
{
    using Players;

    public class DefaultMove : Move
    {
        public DefaultMove(Field field, Player player, FieldMask fieldMask) : base(field, player, fieldMask)
        {
        }

        public override bool IsValid()
        {
            return false;
        }

        public override void Execute()
        {
        }

        public override void Undo()
        {
        }
    }
}
