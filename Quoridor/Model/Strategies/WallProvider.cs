namespace Quoridor.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Players;
    using Strategies;

    public interface IWallProvider
    {
        byte[] GetAllMoves();

        List<byte> GenerateWallMoves(Field field);

        byte[] GenerateWallMoves(Field field, Player player);

        bool CanPlaceWall(Field field, FieldMask wall);

        FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation);

        bool TryGetWallBehind(Field field, Player player, out byte wall);

        bool HasCachedWalls(Field field, Player player, out List<byte> walls);
  
        void SetCachedWalls(Field field, Player player, List<byte> walls);
    }

    public class WallProvider : IWallProvider
    {
        private readonly IMoveProvider moveProvider;
        private static readonly Dictionary<(FieldMask walls, byte player, byte enemy), byte[]> CachedMoves = new();
        private static readonly Dictionary<(FieldMask walls, byte player, byte enemy), List<byte>> CachedWalls = new();

        public WallProvider(IMoveProvider moveProvider)
        {
            this.moveProvider = moveProvider;
        }

        public bool CanPlaceWall(Field field, FieldMask wall)
        {
            // return field.PossibleWalls.Any(w => w == wall);
            return false;
        }

        public FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation)
        {
            return WallConstants.GenerateWall(y, x, wallOrientation);
        }

        public byte[] GetAllMoves()
        {
            return WallConstants.AllIndexes;
        }

        public List<byte> GenerateWallMoves(Field field)
        {
            return field.PossibleWalls;
        }

        public byte[] GenerateWallMoves(Field field, Player player)
        {
            var key = (field.Walls, player.Position, player.Enemy.Position);
            if (CachedMoves.TryGetValue(key, out var walls))
            {
                return walls;
            }

            walls = CreateWallMoves(field, player);
            CachedMoves[key] = walls;
            return walls;
        }

        private byte[] CreateWallMoves(Field field, Player player)
        {
            var heuristicWalls = GetNearWalls(field)
                // .Concat(WallConstants.NearEdgeWalls)
                .Concat(WallConstants.NearPlayerWalls[player.Position])
                .Concat(WallConstants.NearPlayerWalls[player.Enemy.Position]);

            var result = heuristicWalls.Intersect(field.PossibleWalls).Distinct().ToArray();
            return result;
        }

        private IEnumerable<byte> GetNearWalls(Field field)
        {
            return field.PlacedWalls.SelectMany(w => WallConstants.NearWallsToPlace[w]);
        }

        public bool TryGetWallBehind(Field field, Player player, out byte wall)
        {
            var row = moveProvider.GetRow(in player.Position);
            if (row == 0 && player.EndDownIndex == PlayerConstants.EndRedDownIndexIncluding ||
                row == 16 && player.EndDownIndex == PlayerConstants.EndBlueDownIndexIncluding)
            {
                wall = 0;
                return false;
            }

            wall = WallConstants.BehindPlayerWall[(player.Position, player.EndDownIndex)];
            return field.Walls.And(in WallConstants.AllWalls[wall]).IsZero();
        }

        public bool HasCachedWalls(Field field, Player player, out List<byte> walls)
        {
            var key = (field.Walls, player.Position, player.Enemy.Position);
            return CachedWalls.TryGetValue(key, out walls);
        }

        public void SetCachedWalls(Field field, Player player, List<byte> walls)
        {
            var key = (field.Walls, player.Position, player.Enemy.Position);
            CachedWalls[key] = walls.ToList();
        }
    }
}
