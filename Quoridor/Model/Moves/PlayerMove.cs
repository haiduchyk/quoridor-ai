namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public class PlayerMove : IMove
    {
        private readonly Player player;
        private readonly FieldMask position;
        private readonly FieldMask previousPosition;

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
    }
}
