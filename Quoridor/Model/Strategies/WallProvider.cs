namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Players;
    using Strategies;

    public interface IWallProvider
    {
        FieldMask[] GetAllMoves();

        List<FieldMask> GenerateWallMoves(Field field);

        FieldMask[] GenerateWallMoves(Field field, Player player);

        bool CanPlaceWall(Field field, FieldMask wall);

        FieldMask GenerateWall(int y, int x, WallOrientation wallOrientation);

        bool TryGetWallBehind(Field field, Player player, out FieldMask wall);
    }

    public class WallProvider : IWallProvider
    {

        private readonly IMoveProvider moveProvider;
        public static readonly Dictionary<(FieldMask walls, FieldMask player, FieldMask enemy), FieldMask[]> cached = new();

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


        public FieldMask[] GetAllMoves()
        {
            return WallConstants.indexToMask;
        }

        public List<FieldMask> GenerateWallMoves(Field field)
        {
            // TODO index
            // return field.PossibleWalls;
            return null;
        }

        public FieldMask[] GenerateWallMoves(Field field, Player player)
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

        private FieldMask[] CreateWallMoves(Field field, Player player)
        {
            var moves = field.PossibleWalls;
            var nearPlayer = WallConstants.nearPlayerWallsMasks[player.Position];
            var nearEnemy = WallConstants.nearPlayerWallsMasks[player.Enemy.Position];
            var nearWallMask = GetNearWallMask(field);

            // TODO index
            // return moves
            //     .Where(w =>
            //         // w.And(in WallConstants.nearEdgeWallMask).IsNotZero() ||
            //         w.And(in nearPlayer).IsNotZero() ||
            //         w.And(in nearEnemy).IsNotZero() ||
            //         w.And(in nearWallMask).IsNotZero()
            //         )
            //     .ToArray();
            return null;
        }

        private FieldMask GetNearWallMask(Field field)
        {
            return WallConstants.indexToMask.Where(w => w == field.Walls)
                .Aggregate(new FieldMask(), (agg, w) => WallConstants.nearWallsMasks[w].Or(in agg));
        }

        public bool TryGetWallBehind(Field field, Player player, out FieldMask wall)
        {
            var row = moveProvider.GetRow(in player.Position);
            if (row == 0 && player.EndPosition == Constants.RedEndPositions ||
                row == 16 && player.EndPosition == Constants.BlueEndPositions)
            {
                wall = Constants.EmptyField;
                return false;
            }

            wall = WallConstants.behindPlayerWall[(player.Position, player.EndPosition)];
            return field.Walls.And(in wall).IsZero();
        }
    }
}
