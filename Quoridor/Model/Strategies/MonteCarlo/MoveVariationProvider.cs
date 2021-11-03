namespace Quoridor.Model.Strategies
{
    using System.Collections.Generic;
    using System.Linq;
    using Moves;
    using Players;

    public class MoveVariationProvider
    {
        private readonly IMoveProvider moveProvider;
        private readonly IWallProvider wallProvider;
        private readonly ISearch search;
        private readonly Field field;
        private readonly Player player;

        public MoveVariationProvider(IMoveProvider moveProvider, IWallProvider wallProvider, ISearch search,
            Field field, Player player)
        {
            this.moveProvider = moveProvider;
            this.wallProvider = wallProvider;
            this.search = search;
            this.field = field;
            this.player = player;
        }

        public List<IMove> MoveOnPath(MonteNode node)
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

        public List<IMove> FromMove(byte moveMask)
        {
            return new List<IMove>() { new PlayerMove(player, moveMask) };
        }

        public List<IMove> FromWall(byte wall)
        {
            return new List<IMove>() { new WallMove(field, player, search, wall) };
        }

        public List<IMove> Shifts(MonteNode node)
        {
            var turnPlayer = node.IsPlayerMove ? player : player.Enemy;
            var shifts = moveProvider.GetAvailableMoves(field, in turnPlayer.Position, in turnPlayer.Enemy.Position);
            return shifts.Select<byte, IMove>(m => new PlayerMove(turnPlayer, m)).ToList();
        }

        private IEnumerable<IMove> GetWallMoves(MonteNode node)
        {
            var turnPlayer = node.IsPlayerMove ? player : player.Enemy;
            var walls = wallProvider.GenerateWallMoves(field, turnPlayer);
            return walls
                .Select<byte, IMove>(w => new WallMove(field, turnPlayer, search, w))
                .Where(m => m.IsValid());
        }

        public List<IMove> ShiftsWithBlockingWalls(MonteNode node)
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
                .Select<byte, IMove>(w => new WallMove(field, turnPlayer, search, w))
                .Where(m => m.IsValid());
        }

        public List<IMove> AllMoves(MonteNode node)
        {
            var moves = Shifts(node);
            moves.AddRange(GetWallMoves(node));
            return moves;
        }

        public int GetRow(Player player)
        {
            return moveProvider.GetRow(player.Position);
        }

        public bool TryGetWallBehind(Field field, Player player, out byte wall)
        {
            return wallProvider.TryGetWallBehind(field, player, out wall);
        }

        public bool TryMoveForward(Field field, Player player, out byte move)
        {
            return moveProvider.TryMoveForward(field, player, out move);
        }
    }
}
