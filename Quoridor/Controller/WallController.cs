namespace Quoridor.Controller
{
    using Model;

    public class WallController
    {
        private readonly IGameProvider gameProvider;
        private readonly IWallProvider wallProvider;

        public WallController(IGameProvider gameProvider, IWallProvider wallProvider)
        {
            this.gameProvider = gameProvider;
            this.wallProvider = wallProvider;
        }

        public void PlaceWall(int y, int x, WallOrientation wallOrientation)
        {
            var possibleWalls = gameProvider.Game.Field.GetPossibleWallsMask();
            if (wallProvider.CanPlaceWall(ref possibleWalls, y, x, wallOrientation))
            {
                var wall = wallProvider.GenerateWall(y, x, wallOrientation);
                gameProvider.Game.Field.PlaceWall(ref wall);
            }
        }
    }
}
