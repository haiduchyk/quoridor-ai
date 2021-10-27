namespace Quoridor.Model
{
    using Players;

    public interface ISearch
    {
        bool HasPath(Field field, Player player, in FieldMask position, out FieldMask path);
    }
}
