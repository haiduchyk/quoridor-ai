namespace Quoridor.Model
{
    using System;
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
    }

    public class WallProvider : IWallProvider
    {
        private readonly IMoveProvider moveProvider;
        public static readonly Dictionary<(FieldMask walls, byte player, byte enemy), byte[]> cached = new();

        public WallProvider(IMoveProvider moveProvider)
        {
            this.moveProvider = moveProvider;
        }

        public bool CanPlaceWall(Field field, FieldMask wall)
        {
            // TODO index
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
            var combined = (field.Walls, player.Position, player.Enemy.Position);
            if (cached.TryGetValue(combined, out var walls))
            {
                return walls;
            }

            walls = CreateWallMoves(field, player);
            cached[combined] = walls;
            return walls;
        }

        private byte[] CreateWallMoves(Field field, Player player)
        {
            var heuristicWalls = WallConstants.NearEdgeWalls
                .Concat(WallConstants.NearPlayerWalls[player.Position])
                .Concat(WallConstants.NearPlayerWalls[player.Enemy.Position])
                .Concat(GetNearWalls(field));

            return heuristicWalls.Intersect(field.PossibleWalls).Distinct().ToArray();
        }

        private IEnumerable<byte> GetNearWalls(Field field)
        {
            return field.PlacedWalls.SelectMany(w => WallConstants.NearWallsMasks[w]);
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
    }
}
