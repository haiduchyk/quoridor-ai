namespace Quoridor.Model.Moves
{
    using System;
    using Model;
    using Players;
    using Strategies;

    public class PlayerMove : IMove
    {
        public ref readonly byte Id => ref position;

        private readonly byte position;
        private readonly byte previousPosition;
        private Player player;
        private Field field;
        private ISearch search;

        public PlayerMove(Player player, byte position, Field field, ISearch search)
        {
            this.player = player;
            this.position = position;
            this.field = field;
            this.search = search;
            previousPosition = player.Position;
        }

        public void Execute()
        {
            player.ChangePosition(position);
            search.UpdatePathForPlayers(field, player);
            field.MakeMoveAndUpdate(player);
        }

        public void Apply(Field field, Player player)
        {
            this.player = player;
        }
        
        public void Log()
        {
            PlayerConstants.allPositions[Id].Log();
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
