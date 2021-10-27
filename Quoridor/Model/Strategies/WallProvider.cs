namespace Quoridor.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Players;

    public interface IWallProvider
    {
        FieldMask[] GetAllMoves();

        List<FieldMask> GenerateWallMoves(Field field);

        List<FieldMask> GenerateWallMoves(Field field, Player player);

        bool CanPlaceWall(Field field, in FieldMask wall);

        FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation);
    }

    public class WallProvider : IWallProvider
    {
        private const int MaxWallCount = 128;
        private const int WallSize = 3;

        private FieldMask[] allWalls;
        private FieldMask nearEdgeWallMask;
        private readonly Dictionary<FieldMask, FieldMask> nearPlayerWalls = new();
        private readonly Dictionary<FieldMask, FieldMask> nearWalls = new();

        public WallProvider()
        {
            GenerateAllWallMoves();
            GenerateNearEdgeWallMask();
            GenerateNearPlayerWalls();
            GenerateNearWall();
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

        private void GenerateNearEdgeWallMask()
        {
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                nearEdgeWallMask.SetBit(i, 0, true);
                nearEdgeWallMask.SetBit(i, FieldMask.BitboardSize - 1, true);
            }
        }

        private void GenerateNearPlayerWalls()
        {
            for (var i = 0; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 0; j < FieldMask.BitboardSize; j += 2)
                {
                    var position = new FieldMask();
                    position.SetBit(i, j, true);
                    nearPlayerWalls[position] = GetWallMask(i, j);
                }
            }
        }

        private FieldMask GetWallMask(int i, int j)
        {
            var wallMask = new FieldMask();
            foreach (var (offsetY, offsetX) in Constants.Directions)
            {
                var y = i + offsetY;
                var x = j + offsetX;
                wallMask.TrySetBit(y, x, true);
            }
            return wallMask;
        }

        private void GenerateNearWall()
        {
            var count = 0;
            for (var i = 1; i < FieldMask.BitboardSize; i += 2)
            {
                for (var j = 1; j < FieldMask.BitboardSize; j += 2)
                {
                    var wall = allWalls[count++];
                    var mask = new FieldMask();
                    foreach (var (offsetY, offsetX) in Constants.Directions)
                    {
                        mask.TrySetBit(i + offsetY, j + 2 * offsetX, true);
                        mask.TrySetBit(i + 2 * offsetY, j + 3 * offsetX, true);
                    }
                    nearWalls[wall] = mask;
                    wall = allWalls[count++];
                    mask = new FieldMask();
                    foreach (var (offsetY, offsetX) in Constants.Directions)
                    {
                        mask.TrySetBit(i + 2 * offsetY, j + offsetX, true);
                        mask.TrySetBit(i + 3 * offsetY, j + 2 * offsetX, true);
                    }
                    nearWalls[wall] = mask;
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

        public FieldMask[] GetAllMoves()
        {
            return allWalls;
        }

        public List<FieldMask> GenerateWallMoves(Field field)
        {
            return field.PossibleWalls;
        }

        public List<FieldMask> GenerateWallMoves(Field field, Player player)
        {
            var moves = field.PossibleWalls;
            var nearWallMask = GetNearWallMask(field);
            var nearPlayer = nearPlayerWalls[player.Position];
            var nearEnemy = nearPlayerWalls[player.Enemy.Position];
            return moves
                .Where(w =>
                    w.And(in nearEdgeWallMask).IsNotZero() ||
                    w.And(in nearPlayer).IsNotZero() ||
                    w.And(in nearEnemy).IsNotZero() ||
                    w.And(in nearWallMask).IsNotZero())
                .ToList();
        }

        private FieldMask GetNearWallMask(Field field)
        {
            return allWalls.Where(w => w == field.Walls)
                .Aggregate(new FieldMask(), (agg, w) => nearWalls[w].Or(in agg));
        }

        public bool CanPlaceWall(Field field, in FieldMask wall)
        {
            return field.Walls.And(in wall).IsZero();
        }
    }
}
