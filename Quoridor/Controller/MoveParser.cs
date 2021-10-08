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
        private const char Zero = '0';
        private const char Nine = '9';
        private const char A = 'a';
        private const char G = 'g';
        private const string H = "h";
        private const string V = "v";

        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;

        public MoveParser(IMoveProvider moveProvider, IWallProvider wallProvider)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
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

        private int ParseIndex(char character)
        {
            return character switch
            {
                >= Zero and <= Nine => character - Zero,
                >= A and <= G => character - A + 10,
                _ => -1
            };
        }

        private bool CanMoveTo(Field field, Player player, FieldMask to)
        {
            var playerPosition = player.Position;
            var moves = moveProvider.GetAvailableMoves(field, ref playerPosition);
            return moves.Any(m => !m.And(ref to).IsZero());
        }

        private bool TryParseAsWallMove(Field field, Player player, string input, out Move move)
        {
            var (y, x, orientation, isValid) = ParseWallMove(input);
            if (isValid && CanPlace(field, y, x, orientation))
            {
                var wall = wallProvider.GenerateWall(y, x, orientation);
                move = new WallMove(field, player, wall);
                return true;
            }
            move = null;
            return false;
        }

        private (int y, int x, WallOrientation orientation, bool isValid) ParseWallMove(string input)
        {
            var split = input.Trim().Split(' ');
            if (split.Length != 2)
            {
                return (-1, -1, WallOrientation.Horizontal, false);
            }
            if (TryParseCellIndex(split[0], out var y, out var x) && TryWallOrientation(split[1], out var orientation))
            {
                return (y, x, orientation, true);
            }
            return (-1, -1, WallOrientation.Horizontal, false);
        }

        private bool CanPlace(Field field, int y, int x, WallOrientation orientation)
        {
            var possibleWalls = field.GetPossibleWallsMask();
            return wallProvider.CanPlaceWall(ref possibleWalls, y, x, orientation);
        }

        private bool TryParseCell(string input, out FieldMask mask)
        {
            mask = new FieldMask();
            if (TryParseCellIndex(input, out var y, out var x))
            {
                mask.SetBit(y, x, true);
                return true;
            }
            return false;
        }

        private bool TryParseCellIndex(string input, out int y, out int x)
        {
            if (input.Length != 2)
            {
                y = -1;
                x = -1;
                return false;
            }
            x = ParseIndex(input[0]);
            y = ParseIndex(input[1]);
            return FieldMask.IsInRange(y, x);
        }

        private bool TryWallOrientation(string input, out WallOrientation orientation)
        {
            switch (input)
            {
                case H:
                    orientation = WallOrientation.Horizontal;
                    return true;
                case V:
                    orientation = WallOrientation.Vertical;
                    return true;
                default:
                    orientation = WallOrientation.Horizontal;
                    return false;
            }
        }
    }
}
