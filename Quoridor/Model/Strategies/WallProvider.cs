namespace Quoridor.Model
{
    using System.Linq;

    public interface IWallProvider
    {
        FieldMask[] GenerateWallMoves(Field field);

        bool CanPlaceWall(Field field, FieldMask wall);

        FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation);
    }

    public class WallProvider : IWallProvider
    {
        private const int MaxWallCount = 128;
        private const int WallSize = 3;

        private FieldMask[] allWalls;

        public WallProvider()
        {
            GenerateAllWallMoves();
        }

        public void GenerateAllWallMoves()
        {
            allWalls = new FieldMask[MaxWallCount];
            var count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 1; j < FieldMask.BitboardSize; j += 2)
                {
                    allWalls[count++] = GenerateWall(i, j, WallOrientation.Horizontal);
                    allWalls[count++] = GenerateWall(i, j, WallOrientation.Vertical);
                }
            }
        }

        public FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation)
        {
            var (yOffset, xOffset) = GetOffset(wallOrientation);
            var wall = new FieldMask();
            for (var i = 0; i < WallSize; i++)
            {
                var placeY = y + (i - 1) * yOffset;
                var placeX = x + (i - 1) * xOffset;
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

        public FieldMask[] GenerateWallMoves(Field field)
        {
            var walls = field.GetWallsMask();
            return allWalls.Where(w => walls.And(in w).IsZero()).ToArray();
        }

        public bool CanPlaceWall(Field field, FieldMask wall)
        {
            var walls = field.GetWallsMask();
            return walls.And(in wall).IsZero();
        }
    }
}
