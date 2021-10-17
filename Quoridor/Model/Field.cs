namespace Quoridor.Model
{
    public class Field
    {
        public int Size { get; }

        private FieldMask availableWalls;
        public FieldMask walls;

        public Field(int size)
        {
            Size = size;
            CreateAvailableWallsMask();
        }

        private void CreateAvailableWallsMask()
        {
            for (var i = 1; i < Size; i += 2)
            {
                for (var j = 1; j < Size; j += 2)
                {
                    availableWalls.SetBit(i, j, true);
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

        public FieldMask GetPossibleWallsMask()
        {
            return availableWalls.Nor(in walls);
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
    }
}
