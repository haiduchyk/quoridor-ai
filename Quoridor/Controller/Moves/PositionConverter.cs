namespace Quoridor.Controller.Moves
{
    using Model;
    using Model.Strategies;

    public interface IPositionConverter
    {
        byte? TryParseCellPosition(string code);

        byte? TryParseWallPosition(string code);

        string CellPositionToCode(byte position);

        string WallPositionToCode(byte wall);
    }

    public class PositionConverter : IPositionConverter
    {
        private readonly IWallProvider wallProvider;

        public PositionConverter(IWallProvider wallProvider)
        {
            this.wallProvider = wallProvider;
        }

        public byte? TryParseCellPosition(string code)
        {
            var position = TryParse(code, 'a', 8);
            if (!position.HasValue)
            {
                return null;
            }
            var (row, column) = position.Value;
            var (i, j) = ToFieldMaskIndex(row, column);
            return FieldMask.GetPlayerIndex(i, j);
        }

        public byte? TryParseWallPosition(string code)
        {
            var orientation = code.ToLower()[^1];

            if (orientation != 'h' && orientation != 'v')
            {
                return null;
            }

            var wallType = orientation == 'h' ? WallOrientation.Horizontal : WallOrientation.Vertical;

            var position = TryParse(code[0..^1], 's', 7);
            if (!position.HasValue)
            {
                return null;
            }
            var (row, column) = position.Value;
            var (i, j) = ToFieldMaskWallIndex(row, column);
            return WallConstants.ToIndex(i, j, wallType);
        }

        public string CellPositionToCode(byte position)
        {
            var (i, j) = FieldMask.Flatten(position);
            var (row, column) = FromFieldMaskIndex(i, j);
            return $"{(char)('A' + column)}{row + 1}";
        }

        public string WallPositionToCode(byte wall)
        {
            var (i, j, type) = WallConstants.Flatten(wall);
            var (row, column) = FromFieldMaskIndex(i, j);
            return
                $"{(char)('S' + column)}{row + 1}{(type == WallOrientation.Horizontal ? 'h' : 'v')}";
        }

        private (int row, int column)? TryParse(string code, char startSymbol, int limit)
        {
            if (code == null)
            {
                return null;
            }

            code = code.ToLower();
            if (code.Length != 2)
            {
                return null;
            }

            var symbol = code[0];
            var number = code[1];

            var column = symbol - startSymbol;
            var row = number - '1';

            if (row >= 0 && column >= 0 && row <= limit && column <= limit)
            {
                return (row, column);
            }

            return null;
        }

        private (int i, int j) ToFieldMaskIndex(int row, int column)
        {
            return (row * 2, column * 2);
        }

        private (int i, int j) ToFieldMaskWallIndex(int row, int column)
        {
            return (row * 2 + 1, column * 2 + 1);
        }

        private (int row, int column) FromFieldMaskIndex(int i, int j)
        {
            return (i / 2, j / 2);
        }
    }
}
