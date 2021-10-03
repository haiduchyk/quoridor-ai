namespace Quoridor.Logic
{
    public interface ISearch
    {
        bool HasPath(QuoridorModel model, int position, Direction direction, out Path path);
    }
}
