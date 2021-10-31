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
        public static readonly Dictionary<(FieldMask walls, FieldMask player, FieldMask enemy), byte[]> cached = new();

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
            var moves = field.PossibleWalls;
            var nearPlayer = WallConstants.NearPlayerWallsMasks[player.Position];
            var nearEnemy = WallConstants.NearPlayerWallsMasks[player.Enemy.Position];
            var nearWallMask = GetNearWallMask(field);

            return moves
                .Where(b =>
                    WallConstants.AllWalls[b].And(in WallConstants.nearEdgeWallMask).IsNotZero() ||
                    WallConstants.AllWalls[b].And(in nearPlayer).IsNotZero() ||
                    WallConstants.AllWalls[b].And(in nearEnemy).IsNotZero() ||
                    WallConstants.AllWalls[b].And(in nearWallMask).IsNotZero())
                .ToArray();
        }

        private FieldMask GetNearWallMask(Field field)
        {
            return WallConstants.AllWalls.Where(w => w == field.Walls)
                .Aggregate(new FieldMask(), (agg, w) => WallConstants.NearWallsMasks[w].Or(in agg));
        }

        public bool TryGetWallBehind(Field field, Player player, out byte wall)
        {
            var row = moveProvider.GetRow(in player.Position);
            if (row == 0 && player.EndPosition == Constants.RedEndPositions ||
                row == 16 && player.EndPosition == Constants.BlueEndPositions)
            {
                wall = 0;
                return false;
            }

            wall = WallConstants.BehindPlayerWall[(player.Position, player.EndPosition)];
            return field.Walls.And(in WallConstants.AllWalls[wall]).IsZero();
        }
    }
}
