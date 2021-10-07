namespace Quoridor.Controller
{
    using Model;
    using Model.Moves;
    using Model.Players;

    public interface IMoveParser
    {
        Move Parse(Field field, Player player, string input);
    }

    public class MoveParser : IMoveParser
    {
        public Move Parse(Field field, Player player, string input)
        {
            return new DefaultMove(field, player, new FieldMask());
        }
    }
}
