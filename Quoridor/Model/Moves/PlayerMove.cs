namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public class PlayerMove : IMove
    {
        private readonly FieldMask position;
        private readonly FieldMask previousPosition;
        private Player player;

        public PlayerMove(Player player, FieldMask position)
        {
            this.player = player;
            this.position = position;
            previousPosition = player.Position;
        }

        public bool IsValid()
        {
            return true;
        }

        public void Execute()
        {
            player.ChangePosition(position);
        }

        public void Undo()
        {
            player.ChangePosition(previousPosition);
        }

        public void Apply(Field field, Player player, Player enemy)
        {
            this.player = player;
        }
    }
}
