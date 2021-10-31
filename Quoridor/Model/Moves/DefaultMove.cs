namespace Quoridor.Model.Moves
{
    using Players;

    public class DefaultMove : IMove
    {
        public ref readonly FieldMask Id => ref Constants.EmptyField;

        public bool IsValid()
        {
            return false;
        }

        public void Execute()
        {
        }

        public void Apply(Field field, Player player)
        {
        }
    }
}
