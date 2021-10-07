namespace Quoridor.Logic
{
    public class Field
    {
        public int Size { get; }

        private FieldMask availableWalls;
        private FieldMask walls;
        private FieldMask blueCharacter;
        private FieldMask redCharacter;

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

        public void PlaceWall(ref FieldMask wall)
        {
            walls = walls.Or(ref wall);
        }

        public FieldMask GetPossibleWallsMask()
        {
            return availableWalls.Nor(ref walls);
        }

        public bool CanMove(ref FieldMask moveMask)
        {
            return walls.And(ref moveMask).IsZero();
        }

        public int Flatten(int y, int x)
        {
            return x + y * FieldMask.BitboardSize;
        }

        public (int i, int j) Nested(int position)
        {
            var i = position / FieldMask.BitboardSize;
            var j = position % FieldMask.BitboardSize;
            return (i, j);
        }

        public FieldMask GetWallsForMask(ref FieldMask wallMask)
        {
            return wallMask.And(ref walls);
        }
    }
}
