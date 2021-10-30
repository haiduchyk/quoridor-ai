namespace Quoridor.Model.Moves
{
    using System.Collections.Generic;

    public class CloseWallChecker
    {
        // <wall, closeWalls>
        public Dictionary<FieldMask, FieldMask> WallsMasks = new();

        // horizontal and inverted vertical
        private List<(int y, int x)> offsets = new()
        {
            (3, 2),
            (3, -2),
            (-3, 2),
            (-3, -2),
            (1, 2),
            (1, -2),
            (-1, 2),
            (-1, -2),
            (0, 2),
            (0, -2)
        };

        public CloseWallChecker()
        {
        }

        private void CreateWallMasks()
        {
        }
    }
}