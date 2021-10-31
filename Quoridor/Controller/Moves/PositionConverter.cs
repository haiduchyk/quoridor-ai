namespace Quoridor.Controller.Moves
{
    using System;
    using Model;
    using Model.Strategies;

    public interface IPositionConverter
    {
        FieldMask? TryParseCellPosition(string code);

        byte? TryParseWallPosition(string code);

        string CellPositionToCode(FieldMask position);

        string WallPositionToCode(FieldMask wall);
    }

    public class PositionConverter : IPositionConverter
    {
        private readonly IWallProvider wallProvider;

        public PositionConverter(IWallProvider wallProvider)
        {
            this.wallProvider = wallProvider;
        }

        public FieldMask? TryParseCellPosition(string code)
        {
            return TryParse(code, 'a', 8);
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
            var (row, column) = ConvertToPosition(position.Value);
            var (i, j) = ToFieldMaskWallIndex(row, column);
            return WallConstants.ToIndex(i, j, wallType);
        }

        public string CellPositionToCode(FieldMask position)
        {
            var (row, column) = ConvertToPosition(position);
            return $"{(char)('A' + column)}{row + 1}";
        }

        public string WallPositionToCode(FieldMask wall)
        {
            var (row, column) = ConvertToPosition(wall);
            var type = GetWallType(row, column, wall);
            return
                $"{(char)('S' + column)}{row + 1}{(type == WallOrientation.Horizontal ? 'h' : 'v')}";
        }

        private FieldMask? TryParse(string code, char startSymbol, int limit)
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
                var position = new FieldMask();
                var (i, j) = ToFieldMaskIndex(row, column);
                position.SetBit(i, j, true);
                return position;
            }

            return null;
        }

        private (int row, int column) ConvertToPosition(FieldMask mask)
        {
            for (var i = 0; i < FieldMask.BitboardSize; i++)
            {
                for (var j = 0; j < FieldMask.BitboardSize; j++)
                {
                    if (mask.GetBit(i, j))
                    {
                        return FromFieldMaskIndex(i, j);
                    }
                }
            }
            throw new Exception("Invalid field mask in move");
        }

        private WallOrientation GetWallType(int row, int column, FieldMask wall)
        {
            var (i, j) = ToFieldMaskWallIndex(row, column);
            return wall.GetBit(i + 1, j) ? WallOrientation.Vertical : WallOrientation.Horizontal;
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
