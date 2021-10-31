namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;
    using Strategies;

    public class Field
    {
        public List<FieldMask> PossibleWalls { get; } = new();
        public List<FieldMask> ProbableValidWalls { get; } = new();

        public ref readonly FieldMask Walls => ref walls;

        public FieldMask walls;

        public void PlaceWallAndUpdateValidMoves(in FieldMask wall, Player player)
        {
            walls = walls.Or(in wall);
            var nearWalls = WallConstants.nearWalls[wall];

            PossibleWalls.Remove(wall);
            foreach (var nearWall in nearWalls)
            {
                PossibleWalls.Remove(nearWall);
            }
        }
        
        public void PlaceWallAndUpdateProbableValidMoves(in FieldMask wall, Player player)
        {
            walls = walls.Or(in wall);
            for (var i = 0; i < PossibleWalls.Count; i++)
            {
                if (PossibleWalls[i].And(in wall).IsNotZero())
                {
                    PossibleWalls.RemoveAt(i);
                    i--;
                }
            }
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