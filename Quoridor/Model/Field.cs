namespace Quoridor.Model
{
    using System.Collections.Generic;

    public class Field
    {
        public int Size { get; }

        public List<FieldMask> PossibleWalls { get; } = new();

        public ref readonly FieldMask Walls => ref walls;

        private FieldMask walls;

        public Field()
        {
        }

        public Field(int size)
        {
            Size = size;
        }

        public void PlaceWallAndUpdateMoves(in FieldMask wall)
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
            PossibleWalls.Clear();
            PossibleWalls.AddRange(field.PossibleWalls);
        }
    }
}
