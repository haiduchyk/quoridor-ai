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
        public List<byte> PossibleWalls { get; } = new();
        public List<byte> ValidWalls { get; set; } = new();
        public List<byte> ProbableValidWalls { get; } = new();

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
            UpdateValidWalls(in wallIndex, player);
        }

        private void RemoveOverlappedWalls(in byte wallIndex)
        {
            var overlappedWall = WallConstants.OverlappedWalls[wallIndex];
            foreach (var nearWall in overlappedWall)
            {
                PossibleWalls.Remove(nearWall);
            }
        }

        private void UpdateValidWalls(in byte wallIndex, Player player)
        {
            RemoveCrossingWalls(in wallIndex, player);
        }
        // TODO probably will fall if player put wall on first move, ask before remove this todo
        private void RemoveCrossingWalls(in byte wallIndex, Player player)
        {
            ValidWalls = PossibleWalls.ToList();
            var shortestPlayerPath = player.CurrentPath;
            var shortestEnemyPath = player.Enemy.CurrentPath;
            
            var nearWalls = WallConstants.NearWalls[wallIndex];
            foreach (var nearWall in nearWalls)
            {
                var nearWallMask = WallConstants.AllWalls[nearWall];
                var isOnPlayerPath = nearWallMask.Or(shortestPlayerPath).IsNotZero();
                var isOnEnemyPath = nearWallMask.Or(shortestEnemyPath).IsNotZero();
                if (isOnPlayerPath && isOnEnemyPath)
                {
                    if (!CheckBothPaths(wallIndex, player))
                    {
                        ValidWalls.Remove(nearWall);
                    }
                }
                else if (isOnEnemyPath)
                {
                    if (!CheckEnemyPath(wallIndex, player))
                    {
                        ValidWalls.Remove(nearWall);
                    }
                }
                else if (isOnPlayerPath)
                {
                    if (!CheckPlayerPath(wallIndex, player))
                    {
                        ValidWalls.Remove(nearWall);
                    }
                }
            }
        }
        
        private bool CheckBothPaths(in byte wallIndex, Player player)
        {
            PlaceWall(in wallIndex);
            var hasPathForEnemy = search.HasPath(this, player.Enemy, in player.Enemy.Position, out _);
            if (!hasPathForEnemy)
            {
                RemoveWall(in wallIndex);
                return false;
            }
            var hasPathForPlayer = search.HasPath(this, player, in player.Position, out _);
            RemoveWall(in wallIndex);
            return hasPathForPlayer;
        }
        
        private bool CheckPlayerPath(in byte wallIndex, Player player)
        {
            PlaceWall(in wallIndex);
            var hasPathForPlayer = search.HasPath(this, player, in player.Position, out _);
            RemoveWall(in wallIndex);
            return hasPathForPlayer ;
        }
        
        private bool CheckEnemyPath(in byte wallIndex, Player player)
        {
            PlaceWall(in wallIndex);
            var hasPathForEnemy = search.HasPath(this, player.Enemy, in player.Enemy.Position, out _);
            RemoveWall(in wallIndex);
            return hasPathForEnemy;
        }

        public void PlaceWallAndUpdateProbableValidMoves(in FieldMask wall, Player player)
        {
        }

        // private bool IsOnShortestPath()
        // {
        //     return player.CurrentPath.And(wall).IsNotZero();
        // }

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
            
            ValidWalls.Clear();
            ValidWalls.AddRange(field.ValidWalls);
        }
    }
}
