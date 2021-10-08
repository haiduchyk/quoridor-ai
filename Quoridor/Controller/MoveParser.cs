namespace Quoridor.Controller
{
    using System.Globalization;
    using System.Linq;
    using Model;
    using Model.Moves;
    using Model.Players;

    public interface IMoveParser
    {
        Move Parse(Field field, Player player, string input);
    }

    public class MoveParser : IMoveParser
    {
        private readonly IMoveProvider moveProvider;

        public MoveParser(IMoveProvider moveProvider)
        {
            this.moveProvider = moveProvider;
        }

        public Move Parse(Field field, Player player, string input)
        {
            if (TryParseAsPlayerMove(field, player, input, out var move))
            {
                return move;
            }
            if (TryParseAsWallMove(field, player, input, out move))
            {
                return move;
            }
            return new DefaultMove(field, player, new FieldMask());
        }

        private bool TryParseAsPlayerMove(Field field, Player player, string input, out Move move)
        {
            var (from, to, isValid) = ParsePlayerMove(input);
            if (isValid && !player.Position.And(ref from).IsZero() && CanMoveTo(field, player, to))
            {
                move = new PlayerMove(field, player, to);
                return true;
            }
            move = null;
            return false;
        }

        private bool CanMoveTo(Field field, Player player, FieldMask to)
        {
            var playerPosition = player.Position;
            var moves = moveProvider.GetAvailableMoves(field, ref playerPosition);
            return moves.Any(m => !m.And(ref to).IsZero());
        }

        private (FieldMask from, FieldMask to, bool isValid) ParsePlayerMove(string input)
        {
            var split = input.Trim().Split(' ');
            if (split.Length != 2)
            {
                return (new FieldMask(), new FieldMask(), false);
            }
            if (TryParseCell(split[0], out var from) && TryParseCell(split[1], out var to))
            {
                return (from, to, true);
            }
            return (new FieldMask(), new FieldMask(), false);
        }

        private bool TryParseCell(string input, out FieldMask mask)
        {
            mask = new FieldMask();
            if (input.Length != 2)
            {
                return false;
            }
            var x = ParseIndex(input[0]);
            var y = ParseIndex(input[1]);
            if (FieldMask.IsInRange(y, x))
            {
                mask.SetBit(y, x, true);
                return true;
            }
            return false;
        }

        private int ParseIndex(char character)
        {
            if (character >= 48 && character <= 57)
            {
                return character - 48;
            }
            if (character >= 97 && character <= 103)
            {
                return character - 97 + 10;
            }
            return -1;
        }

        private bool TryParseAsWallMove(Field field, Player player, string input, out Move move)
        {
            move = null;
            return false;
        }
    }
}
