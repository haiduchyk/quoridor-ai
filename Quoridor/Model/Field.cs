namespace Quoridor.Model
{
    using System.Collections.Generic;
    using System.Linq;

    public class Field
    {
        public int Size { get; }

        public List<FieldMask> PossibleWalls { get; private init; }

        private FieldMask walls;

        public Field(int size)
        {
            Size = size;
            PossibleWalls = new List<FieldMask>();
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

        public FieldMask GetWallsMask()
        {
            return walls;
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

        public Field Copy()
        {
            return new Field(Size)
            {
                walls = walls,
                PossibleWalls = PossibleWalls.ToList(),
            };
        }

        public void Update(Field field)
        {
            walls = field.walls;
            PossibleWalls.Clear();
            PossibleWalls.AddRange(field.PossibleWalls);
        }
    }
}
