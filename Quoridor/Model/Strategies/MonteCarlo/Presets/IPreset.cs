namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;
    using Moves;
    using Players;

    public interface IPreset
    {
        bool TryGetNextMove(MonteNode node, out List<IMove> moves);

        bool IsExpired(MonteNode node);
    }

    public abstract class Preset : IPreset
    {
        protected readonly MoveVariationProvider moveVariationProvider;
        protected readonly Field field;
        protected readonly Player player;

        public Preset(MoveVariationProvider moveVariationProvider, Field field, Player player)
        {
            this.moveVariationProvider = moveVariationProvider;
            this.field = field;
            this.player = player;
        }

        public abstract bool TryGetNextMove(MonteNode node, out List<IMove> moves);

        public abstract bool IsExpired(MonteNode node);

        protected bool IsNthMove(MonteNode node, int n)
        {
            return node.IsPlayerMove && player.NumberOfMoves == n;
        }

        protected bool IsLessNthMove(MonteNode node, int n)
        {
            return node.IsPlayerMove && player.NumberOfMoves < n;
        }

        protected bool IsGreaterNthMove(MonteNode node, int n)
        {
            return node.IsPlayerMove && player.NumberOfMoves > n;
        }

        protected bool IsOnRow(Player player, int n)
        {
            var row = GetRow(player);
            var startRow = GetStartRow(player);
            return Math.Abs(startRow - row) == n;
        }

        protected bool IsLessRow(Player player, int n)
        {
            var row = GetRow(player);
            var startRow = GetStartRow(player);
            return Math.Abs(startRow - row) < n;
        }

        protected bool IsGreaterRow(Player player, int n)
        {
            var row = GetRow(player);
            var startRow = GetStartRow(player);
            return Math.Abs(startRow - row) > n;
        }

        protected int GetRow(Player player)
        {
            return moveVariationProvider.GetRow(player);
        }

        protected int GetStartRow(Player player)
        {
            return player.EndDownIndex == PlayerConstants.EndBlueDownIndexIncluding ? FieldMask.PlayerFieldSize - 1 : 0;
        }

        protected Player TurnPlayer(MonteNode node)
        {
            return node.IsPlayerMove ? player : player.Enemy;
        }

        protected Player TurnEnemy(MonteNode node)
        {
            return node.IsPlayerMove ? player.Enemy : player;
        }
    }
}
