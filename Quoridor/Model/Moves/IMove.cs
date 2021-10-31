namespace Quoridor.Model.Moves
{
    using Players;

    public interface IMove
    {
        bool IsValid();

        void Execute();

        void Apply(Field field, Player player);

        FieldMask GetIdentifier { get; }
        
        public void Log()
        {
            GetIdentifier.Log();
        }
    }
}
