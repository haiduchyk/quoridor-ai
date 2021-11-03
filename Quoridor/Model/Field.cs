namespace Quoridor.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Moves;
    using Players;
    using Strategies;

    public class Field
    {
        public List<byte> PlacedWalls { get; } = new();

        // TODO add check if there are anything, ask before removing this todo
        public List<byte> PossibleWalls { get; set; } = new();

        public ref readonly FieldMask Walls => ref walls;

        private FieldMask walls;
        private readonly ISearch search;

        public Field(ISearch search)
        {
            this.search = search;
        }

        public void PlaceWallAndUpdateValidMoves(in byte wallIndex, Player player)
        {
            PlacedWalls.Add(wallIndex);
            PossibleWalls.Remove(wallIndex);

            var wallMask = WallConstants.AllWalls[wallIndex];
            walls = walls.Or(in wallMask);

            if (!player.HasWalls() && !player.Enemy.HasWalls())
            {
                return;
            }

            RemoveOverlappedWalls(wallIndex);
            RemoveCrossingWalls(in wallIndex, player);
        }

        private void RemoveOverlappedWalls(in byte wallIndex)
        {
            var overlappedWall = WallConstants.OverlappedWalls[wallIndex];
            foreach (var nearWall in overlappedWall)
            {
                PossibleWalls.Remove(nearWall);
            }
        }

        private void RemoveCrossingWalls(in byte wallIndex, Player player)
        {
            var nearWalls = WallConstants.NearWalls[wallIndex];
            foreach (var nearWall in nearWalls)
            {
                TryToRemoveNearWall(nearWall, player);
            }
        }

        private void TryToRemoveNearWall(byte nearWall, Player player)
        {
            var shortestPlayerPath = player.CurrentPath;
            var shortestEnemyPath = player.Enemy.CurrentPath;
            var nearWallMask = WallConstants.AllWalls[nearWall];

            var isOnPlayerPath = nearWallMask.And(shortestPlayerPath).IsNotZero();
            var isOnEnemyPath = nearWallMask.And(shortestEnemyPath).IsNotZero();

            if (isOnPlayerPath && isOnEnemyPath)
            {
                if (!CheckBothPaths(nearWall, player))
                {
                    PossibleWalls.Remove(nearWall);
                }
            }
            else if (isOnEnemyPath)
            {
                if (!CheckSinglePath(nearWall, player.Enemy))
                {
                    PossibleWalls.Remove(nearWall);
                }
            }
            else if (isOnPlayerPath)
            {
                if (!CheckSinglePath(nearWall, player))
                {
                    PossibleWalls.Remove(nearWall);
                }
            }
        }

        private bool CheckBothPaths(in byte wallIndex, Player player)
        {
            var oldWalls = walls;
            PlaceWall(in wallIndex);
            var hasPathForEnemy = search.HasPath(this, player.Enemy, in player.Enemy.Position, out _);
            if (!hasPathForEnemy)
            {
                walls = oldWalls;
                return false;
            }

            var hasPathForPlayer = search.HasPath(this, player, in player.Position, out _);
            walls = oldWalls;
            return hasPathForPlayer;
        }

        private bool CheckSinglePath(in byte wallIndex, Player player)
        {
            var oldWalls = walls;
            PlaceWall(in wallIndex);
            var hasPathForPlayer = search.HasPath(this, player, in player.Position, out _);
            walls = oldWalls;
            return hasPathForPlayer;
        }

        public void MakeMoveAndUpdate(Player player)
        {
            var nearWalls = WallConstants.NearPlayerWalls[player.Position];
            foreach (var nearWall in nearWalls)
            {
                TryToRemoveNearWall(nearWall, player);
            }
        }

        private void PlaceWall(in byte wall)
        {
            walls = walls.Or(in WallConstants.AllWalls[wall]);
        }

        public void RemoveWall(in byte wall)
        {
            walls = walls.Nor(in WallConstants.AllWalls[wall]);
        }

        public bool HasWall(int y, int x)
        {
            return walls.GetBit(y, x);
        }

        public FieldMask GetWallsForMask(in FieldMask wallMask)
        {
            return walls.And(in wallMask);
        }

        public void Update(Field field)
        {
            walls = field.walls;

            PlacedWalls.Clear();
            PlacedWalls.AddRange(field.PlacedWalls);

            PossibleWalls.Clear();
            PossibleWalls.AddRange(field.PossibleWalls);
        }
    }
}