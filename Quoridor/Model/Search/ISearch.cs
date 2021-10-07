namespace Quoridor.Model
{
    public interface ISearch
    {
        bool HasPath(int position, Direction direction, out Path path);
    }
}
