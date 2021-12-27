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
        public List<byte> PossibleWalls { get; private set; } = new();

        public ref readonly FieldMask Walls => ref walls;

        private FieldMask walls;
        // TODO make private
        public readonly ISearch search;

        public Field(ISearch search)
        {
            this.search = search;
        }

        public void PlaceWallAndUpdatePossibleWalls(in byte wallIndex, Player player)
        {
            PlaceWallPermanently(in wallIndex);

            if (!player.HasWalls() && !player.Enemy.HasWalls())
            {
                return;
            }

            RemoveOverlappedWalls(wallIndex);
        }

        public void SetValidWalls(List<byte> possibleWalls)
        {
            PossibleWalls.Clear();
            PossibleWalls.AddRange(possibleWalls);
        }

        public void UpdateValidWalls(in byte wallIndex, Player player)
        {
            if (!player.HasWalls() && !player.Enemy.HasWalls())
            {
                return;
            }
            RemoveCrossingWalls(in wallIndex, player);
        }

        private void PlaceWallPermanently(in byte wallIndex)
        {
            PlacedWalls.Add(wallIndex);
            PossibleWalls.Remove(wallIndex);

            var wallMask = WallConstants.AllWalls[wallIndex];
            walls = walls.Or(in wallMask);
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
            var wallsToCheck = PlacedWalls
                .SelectMany(w => WallConstants.NearWallsToCheck[w])
                .Intersect(PossibleWalls)
                .Distinct();
            foreach (var nearWall in wallsToCheck)
            {
                TryToRemoveNearWall(nearWall, player);
            }
        }

        private void TryToRemoveNearWall(byte wall, Player player)
        {
            var nearWalls = WallConstants.NearWallsToCheck[wall].Intersect(PlacedWalls).Count();
            // TODO vertical walls check
            nearWalls += WallConstants.NearEdgeWalls.Contains(wall) ? 1 : 0;
            if (nearWalls < 2)
            {
                return;
            }

            var mask = WallConstants.AllWalls[wall];

            var isOnPlayerPath = mask.And(player.Path).IsNotZero();
            var isOnEnemyPath = mask.And(player.Enemy.Path).IsNotZero();

            if (isOnPlayerPath && isOnEnemyPath)
            {
                if (!CheckBothPaths(wall, player))
                {
                    PossibleWalls.Remove(wall);
                }
            }
            else if (isOnEnemyPath)
            {
                if (!CheckSinglePath(wall, player.Enemy))
                {
                    PossibleWalls.Remove(wall);
                }
            }
            else if (isOnPlayerPath)
            {
                if (!CheckSinglePath(wall, player))
                {
                    PossibleWalls.Remove(wall);
                }
            }
        }

        private bool CheckBothPaths(in byte wallIndex, Player player)
        {
            var oldWalls = walls;
            PlaceWall(in wallIndex);
            var hasPathForEnemy = search.HasPath(this, player.Enemy, in player.Enemy.Position);
            if (!hasPathForEnemy)
            {
                walls = oldWalls;
                return false;
            }

            var hasPathForPlayer = search.HasPath(this, player, in player.Position);
            walls = oldWalls;
            return hasPathForPlayer;
        }

        private bool CheckSinglePath(in byte wallIndex, Player player)
        {
            var oldWalls = walls;
            PlaceWall(in wallIndex);
            var hasPathForPlayer = search.HasPath(this, player, in player.Position);
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

        public void PlaceWall(in byte wall)
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
