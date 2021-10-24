namespace Quoridor.Model
{
    public class Field
    {
        public int Size { get; }

        private FieldMask walls;

        public Field(int size)
        {
            Size = size;
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
                walls = walls
            };
        }

        public void Update(Field field)
        {
            walls = field.walls;
        }
    }
}
