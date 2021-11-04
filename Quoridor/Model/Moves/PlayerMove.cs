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
        private readonly ISearch search;
        private readonly IWallProvider wallProvider;
        private Player player;
        private Field field;

        public PlayerMove(Player player, byte position, Field field, ISearch search, IWallProvider wallProvider)
        {
            this.player = player;
            this.position = position;
            this.field = field;
            this.search = search;
            this.wallProvider = wallProvider;
        }

        public bool IsValid()
        {
            return true;
        }

        public void Execute()
        {
            player.ChangePosition(position);
            search.UpdatePathForPlayers(field, player);
            if (wallProvider.HasCachedWalls(field, player, out var walls))
            {
                field.SetValidWalls(walls);
            }
            else
            {
                field.MakeMoveAndUpdate(player);
                wallProvider.SetCachedWalls(field, player, field.PossibleWalls);
            }
        }

        public void ExecuteForSimulation()
        {
            player.ChangePosition(position);
        }

        public void Apply(Field field, Player player)
        {
            this.field = field;
            this.player = player;
        }

        public void Log()
        {
            PlayerConstants.allPositions[Id].Log();
        }

        protected bool Equals(PlayerMove other)
        {
            return position.Equals(other.position);
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
            return position;
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
