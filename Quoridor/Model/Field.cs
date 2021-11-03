namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;
    using Strategies;

    public class Field
    {
        public List<byte> PlacedWalls { get; } = new();
        public List<byte> PossibleWalls { get; } = new();
        public List<byte> ValidWalls { get; } = new();
        public List<byte> ProbableValidWalls { get; } = new();

        public ref readonly FieldMask Walls => ref walls;

        private FieldMask walls;

        public void PlaceWallAndUpdateValidMoves(in byte wallIndex, Player player)
        {
            PlacedWalls.Add(wallIndex);
            var wallMask = WallConstants.AllWalls[wallIndex];
            walls = walls.Or(in wallMask);

            var nearWalls = WallConstants.OverlapedWalls[wallIndex];

            PossibleWalls.Remove(wallIndex);
            foreach (var nearWall in nearWalls)
            {
                PossibleWalls.Remove(nearWall);
            }
        }

        public void PlaceWallAndUpdateProbableValidMoves(in FieldMask wall, Player player)
        {
        }

        // private bool IsOnShortestPath()
        // {
        //     return player.CurrentPath.And(wall).IsNotZero();
        // }

        public void PlaceWall(in byte wall)
        {
            walls = walls.Or(in WallConstants.AllWalls[wall]);
        }

        public void RemoveWall(in byte wall)
        {
            walls = walls.Nor(in WallConstants.AllWalls[wall]);
        }

        public bool HasWall(int y, int x)
        {
            return walls.GetBit(y, x);
        }

        public bool HasWall(in byte wallIndex)
        {
            return PlacedWalls.Contains(wallIndex);
        }

        public bool CanPlace(byte wallIndex)
        {
            return PossibleWalls.Contains(wallIndex);
        }

        public FieldMask GetWallsForMask(in FieldMask wallMask)
        {
            return walls.And(in wallMask);
        }

        public void Update(Field field)
        {
            walls = field.walls;
            PlacedWalls.Clear();
            PlacedWalls.AddRange(field.PlacedWalls);
            PossibleWalls.Clear();
            PossibleWalls.AddRange(field.PossibleWalls);
        }
    }
}
