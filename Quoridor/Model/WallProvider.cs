namespace Quoridor.Model
{
    using System.Collections.Generic;

    public interface IWallProvider
    {
        List<FieldMask> GenerateWallMoves(Field field);

        bool CanPlaceWall(ref FieldMask mask, int y, int x, WallOrientation wallOrientation);

        FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation);
    }

    public class WallProvider : IWallProvider
    {
        private const int MaxWallCount = 128;
        private const int WallSize = 3;

        public List<FieldMask> GenerateWallMoves(Field field)
        {
            var possibleWalls = field.GetPossibleWallsMask();
            var generatedWalls = new List<FieldMask>(MaxWallCount);
            for (var i = 1; i < field.Size; i += 2)
            {
                for (var j = 1; j < field.Size; j += 2)
                {
                    if (CanPlaceWall(ref possibleWalls, i, j, WallOrientation.Horizontal))
                    {
                        var wall = GenerateWall(i, j, WallOrientation.Horizontal);
                        generatedWalls.Add(wall);
                    }
                    if (CanPlaceWall(ref possibleWalls, i, j, WallOrientation.Horizontal))
                    {
                        var wall = GenerateWall(i, j, WallOrientation.Vertical);
                        generatedWalls.Add(wall);
                    }
                }
            }

            return generatedWalls;
        }

        public bool CanPlaceWall(ref FieldMask mask, int y, int x, WallOrientation wallOrientation)
        {
            var (yOffset, xOffset) = GetOffset(wallOrientation);
            return mask.GetBit(y, x) &&
                   !mask.GetBit(y + yOffset, x + xOffset) &&
                   !mask.GetBit(y - yOffset, x - xOffset);
        }

        public FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation)
        {
            var (yOffset, xOffset) = GetOffset(wallOrientation);
            var wall = new FieldMask();
            for (var i = 0; i < WallSize; i++)
            {
                var placeY = y + i - yOffset;
                var placeX = x + i - xOffset;
                wall.SetBit(placeY, placeX, true);
            }

            return wall;
        }

        private (int yOffset, int xOffset) GetOffset(WallOrientation wallOrientation)
        {
            var yOffset = wallOrientation == WallOrientation.Vertical ? 1 : 0;
            var xOffset = wallOrientation == WallOrientation.Horizontal ? 1 : 0;
            return (yOffset, xOffset);
        }
    }
}
