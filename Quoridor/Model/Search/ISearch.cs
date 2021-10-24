namespace Quoridor.Model
{
    using Players;

    public interface ISearch
    {
        bool HasPath(Field field, Player player, FieldMask position, out FieldMask path);

        void Initialize(Game game);
    }
}
