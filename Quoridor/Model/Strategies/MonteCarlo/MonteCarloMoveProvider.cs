namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moves;
    using Players;

    public class MonteCarloMoveProvider
    {
        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;
        private readonly Field field;
        private readonly Player player;

        public MonteCarloMoveProvider(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search,
            Field field, Player player)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
            this.field = field;
            this.player = player;
        }

        public List<IMove> FindMoves(MonteNode node)
        {
            var row = moveProvider.GetRow(in player.Position);
            var startRow = player.EndDownIndex == PlayerConstants.EndBlueDownIndexIncluding ? FieldMask.PlayerFieldSize - 1 : 0;
            if (IsLessNthMove(node, 5) && Math.Abs(startRow - row) < 3)
            {
                return MoveOnPath(node);
            }
            if (IsNthMove(node, 3) && Math.Abs(startRow - row) == 3 &&
                wallProvider.TryGetWallBehind(field, player, out var wall))
            {
                return FromWall(wall);
            }
            var turnPlayer = node.IsPlayerMove ? player : player.Enemy;
            var turnEnemy = node.IsPlayerMove ? player : player.Enemy;
            if (turnPlayer.HasReachedFinish())
            {
                return new List<IMove>();
            }
            if (!turnPlayer.HasWalls() && !turnEnemy.HasWalls())
            {
                return MoveOnPath(node);
            }
            if (!turnPlayer.HasWalls())
            {
                return Shifts(node);
            }
            if (node.IsPlayerMove && !player.Enemy.HasWalls())
            {
                return ShiftsWithBlockingWalls(node);
            }
            return AllMoves(node);
        }

        private bool IsLessNthMove(MonteNode node, int n)
        {
            return node.IsPlayerMove && player.NumberOfMoves < n;
        }

        private bool IsNthMove(MonteNode node, int n)
        {
            return node.IsPlayerMove && player.NumberOfMoves == n;
        }

        private List<IMove> MoveOnPath(MonteNode node)
        {
            var turnPlayer = node.IsPlayerMove ? player : player.Enemy;
            if (search.HasPath(field, turnPlayer, in turnPlayer.Position, out var path))
            {
                var moves = moveProvider.GetAvailableMoves(field, in turnPlayer.Position, in turnPlayer.Enemy.Position);
                var movesOnPath = moves.Where(m => PlayerConstants.allPositions[m].And(in path).IsNotZero()).ToArray();
                var shift = movesOnPath.First();
                return FromMove(shift);
            }
            return Shifts(node);
        }

        private List<IMove> FromMove(byte moveMask)
        {
            return new List<IMove>() { new PlayerMove(player, moveMask, field, search) };
        }

        private List<IMove> FromWall(byte wall)
        {
            return new List<IMove>() { new WallMove(field, player, search, wall) };
        }

        private List<IMove> Shifts(MonteNode node)
        {
            var turnPlayer = node.IsPlayerMove ? player : player.Enemy;
            var shifts = moveProvider.GetAvailableMoves(field, in turnPlayer.Position, in turnPlayer.Enemy.Position);
            return shifts.Select<byte, IMove>(m => new PlayerMove(turnPlayer, m, field, search)).ToList();
        }

        private IEnumerable<IMove> GetWallMoves(MonteNode node)
        {
            var turnPlayer = node.IsPlayerMove ? player : player.Enemy;
            var walls = wallProvider.GenerateWallMoves(field, turnPlayer);
            return walls
                .Select<byte, IMove>(w => new WallMove(field, turnPlayer, search, w));
        }

        private List<IMove> AllMoves(MonteNode node)
        {
            var moves = Shifts(node);
            moves.AddRange(GetWallMoves(node));
            return moves;
        }

        private List<IMove> ShiftsWithBlockingWalls(MonteNode node)
        {
            var moves = MoveOnPath(node);
            moves.AddRange(GetBlockingWallMoves(node));
            return moves;
        }

        private IEnumerable<IMove> GetBlockingWallMoves(MonteNode node)
        {
            var turnPlayer = node.IsPlayerMove ? player : player.Enemy;
            var walls = wallProvider.GenerateWallMoves(field);
            search.HasPath(field, turnPlayer.Enemy, in turnPlayer.Enemy.Position, out var path);
            return walls
                .Where(b => WallConstants.AllWalls[b].And(in path).IsNotZero())
                .Select<byte, IMove>(w => new WallMove(field, turnPlayer, search, w));
        }
    }
}
