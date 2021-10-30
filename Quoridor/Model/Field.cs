namespace Quoridor.Model
{
    using System.Collections.Generic;
    using Players;

    public class Field
    {
        public int Size { get; }

        public List<FieldMask> ValidWalls { get; } = new();
        public List<FieldMask> ProbableValidWalls { get; } = new();

        public ref readonly FieldMask Walls => ref walls;

        private FieldMask walls;

        public Field()
        {
        }

        public Field(int size)
        {
            Size = size;
        }

        public void PlaceWallAndUpdateMoves(in FieldMask wall, Player player)
        {
            walls = walls.Or(in wall);
            for (var i = 0; i < ValidWalls.Count; i++)
            {
                if (ValidWalls[i].And(in wall).IsNotZero())
                {
                    ValidWalls.RemoveAt(i);
                    i--;
                }
            }
        }
        
        
        public void PlaceWallAndUpdateValidMoves(in FieldMask wall, Player player)
        {
            walls = walls.Or(in wall);
            for (var i = 0; i < ValidWalls.Count; i++)
            {
                if (ValidWalls[i].And(in wall).IsNotZero())
                {
                    ValidWalls.RemoveAt(i);
                    i--;
                }
            }
        }
        
        public void PlaceWallAndUpdatePropableValidMoves(in FieldMask wall, Player player)
        {
            walls = walls.Or(in wall);
            for (var i = 0; i < ValidWalls.Count; i++)
            {
                if (ValidWalls[i].And(in wall).IsNotZero())
                {
                    ValidWalls.RemoveAt(i);
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

        public int Flatten(int y, int x)
        {
            return x + y * Size;
        }

        public (int i, int j) Nested(int position)
        {
            var i = position / Size;
            var j = position % Size;
            return (i, j);
        }

        public void Update(Field field)
        {
            walls = field.walls;
            ValidWalls.Clear();
            ValidWalls.AddRange(field.ValidWalls);
        }
    }
}
