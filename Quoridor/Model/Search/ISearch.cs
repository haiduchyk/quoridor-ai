namespace Quoridor.Model
{
    using Players;

    public interface ISearch
    {
        bool HasPath(Field field, Player player, in byte position);

        bool TryFindPath(Field field, Player player, in byte position, out FieldMask path);

        void UpdatePathForPlayers(Field field, Player player);

        public ISearch Copy();
    }
}