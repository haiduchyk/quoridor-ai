namespace Quoridor.Model.Moves
{
    using System;
    using Model;
    using Players;

    public class PlayerMove : IMove
    {
        public FieldMask GetIdentifier => position;

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

        public void Apply(Field field, Player player)
        {
            this.player = player;
        }

        protected bool Equals(PlayerMove other)
        {
            return position.Equals(other.position) && previousPosition.Equals(other.previousPosition);
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((PlayerMove)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(position, previousPosition);
        }

        public static bool operator ==(PlayerMove left, PlayerMove right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PlayerMove left, PlayerMove right)
        {
            return !Equals(left, right);
        }
    }
}
