namespace Quoridor.Controller
{
    using Logic;

    public class WallController
    {
        private readonly IWallProvider wallProvider;
        private readonly Field field;

        public WallController(IWallProvider wallProvider, Field field)
        {
            this.wallProvider = wallProvider;
            this.field = field;
        }

        public void PlaceWall(int y, int x, WallOrientation wallOrientation)
        {
            var possibleWalls = field.GetPossibleWallsMask();
            if (wallProvider.CanPlaceWall(ref possibleWalls, y, x, wallOrientation))
            {
                var wall = wallProvider.GenerateWall(y, x, wallOrientation);
                field.PlaceWall(ref wall);
            }
        }
    }
}
