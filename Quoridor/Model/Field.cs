namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;
    using Strategies;

    public class Field
    {
        public List<byte> PossibleWalls { get; } = new();
        public List<byte> ValidWalls { get; } = new();
        public List<byte> ProbableValidWalls { get; } = new();

        public ref readonly FieldMask Walls => ref walls;

        private FieldMask walls;

        public void PlaceWallAndUpdateValidMoves(in byte wallIndex, Player player)
        {
            var wallMask = WallConstants.AllWalls[wallIndex];
            walls = walls.Or(in wallMask);

            var nearWalls = WallConstants.NearWalls[wallIndex];

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

        public void PlaceWall(in FieldMask wall)
        {
            walls = walls.Or(in wall);
        }

        public void RemoveWall(in FieldMask wall)
        {
            walls = walls.Nor(in wall);
        }

        public bool HasWall(int y, int x)
        {
            return walls.GetBit(y, x);
        }

        public FieldMask GetWallsForMask(in FieldMask wallMask)
        {
            return walls.And(in wallMask);
        }

        public void Update(Field field)
        {
            walls = field.walls;
            PossibleWalls.Clear();
            PossibleWalls.AddRange(field.PossibleWalls);
        }
    }
}
