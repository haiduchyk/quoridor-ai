namespace Quoridor.Model.Moves
{
    using Model;
    using Players;

    public abstract class Move
    {
        protected readonly Field field;
        protected readonly Player player;
        protected FieldMask fieldMask;

        public Move(Field field, Player player, FieldMask fieldMask)
        {
            this.field = field;
            this.player = player;
            this.fieldMask = fieldMask;
        }

        public abstract bool IsValid();

        public abstract void Execute();

        public abstract void Undo();
    }
}
