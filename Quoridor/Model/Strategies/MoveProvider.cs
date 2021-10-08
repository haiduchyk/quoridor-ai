namespace Quoridor.Model
{
    using System;
    using System.Collections.Generic;

    public interface IMoveProvider
    {
        FieldMask[] GetAvailableMoves(Field field, ref FieldMask playerMask);
    }

    public class MoveProvider : IMoveProvider
    {
        private const int SimpleMoveBitsAmount = 3;
        private const int MoveWithEnemyBitsAmount = 7;

        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>> simplePlayersMoves = new();
        private Dictionary<FieldMask, FieldMask> simplePlayersMovesMasks = new();

        public MoveProvider()
        {
            CreateSimplePlayerMovesMasks();
            CreateSimplePlayerMoves();
        }

        private void CreateSimplePlayerMovesMasks()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);
                        simplePlayersMovesMasks[playerMask] = CreateSimplePlayerMovesMaskFor(y, x);
                    }
                }
            }
        }

        private FieldMask CreateSimplePlayerMovesMaskFor(int y, int x)
        {
            var fieldMask = new FieldMask();
            for (var yDelta = -SimpleMoveBitsAmount / 2; yDelta < SimpleMoveBitsAmount / 2 + 1; yDelta++)
            {
                for (var xDelta = -SimpleMoveBitsAmount / 2; xDelta < SimpleMoveBitsAmount / 2 + 1; xDelta++)
                {
                    var curY = y + yDelta;
                    var curX = x + xDelta;
                    CalculateSimplePlayerMovesMaskFor(curY, curX, ref fieldMask);
                }
            }

            return fieldMask;
        }

        private void CalculateSimplePlayerMovesMaskFor(int curY, int curX, ref FieldMask fieldMask)
        {
            if (curY % 2 != 0)
            {
                if (curX % 2 == 0)
                {
                    fieldMask.TrySetBit(curY, curX, true);
                }
            }
            else
            {
                if (curX % 2 != 0)
                {
                    fieldMask.TrySetBit(curY, curX, true);
                }
            }
        }

        private void CreateSimplePlayerMoves()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y++)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x++)
                {
                    if (y % 2 == 0 && x % 2 == 0)
                    {
                        var playerMask = new FieldMask();
                        playerMask.SetBit(y, x, true);
                        var moves = CalculateUniqueVariantFor(y, x);
                        simplePlayersMoves[playerMask] = moves;
                    }
                }
            }
        }

        private Dictionary<FieldMask, FieldMask[]> CalculateUniqueVariantFor(int y, int x)
        {
            var result = new Dictionary<FieldMask, FieldMask[]>();

            var uniqueVariants = Math.Pow(4, 2);

            for (var unique = 0; unique < uniqueVariants; unique++)
            {
                var playerMoveMasks = new List<FieldMask>();
                var fieldMask = new FieldMask();

                var uniquePosition = 0;
                for (var yDelta = -SimpleMoveBitsAmount / 2; yDelta < SimpleMoveBitsAmount / 2 + 1; yDelta++)
                {
                    for (var xDelta = -SimpleMoveBitsAmount / 2; xDelta < SimpleMoveBitsAmount / 2 + 1; xDelta++)
                    {
                        var curY = y + yDelta;
                        var curX = x + xDelta;

                        if (curY % 2 != 0)
                        {
                            if (curX % 2 == 0)
                            {
                                UpdateFieldMask();
                            }
                        }
                        else
                        {
                            if (curX % 2 != 0)
                            {
                                UpdateFieldMask();
                            }
                        }

                        void UpdateFieldMask()
                        {
                            var isTrue = (1 << uniquePosition & unique) != 0;
                            if (FieldMask.IsInRange(curY, curX))
                            {
                                fieldMask.SetBit(curY, curX, isTrue);

                                if (!isTrue)
                                {
                                    var newPlayerX = x + xDelta * 2;
                                    var newPlayerY = y + yDelta * 2;

                                    if (FieldMask.IsInRange(newPlayerY, newPlayerX))
                                    {
                                        var playerMask = new FieldMask();
                                        playerMask.SetBit(newPlayerY, newPlayerX, true);
                                        playerMoveMasks.Add(playerMask);
                                    }
                                }
                            }

                            uniquePosition++;
                        }
                    }
                }

                result[fieldMask] = playerMoveMasks.ToArray();
            }

            return result;
        }

        public FieldMask[] GetAvailableMoves(Field field, ref FieldMask playerMask)
        {
            var moves = simplePlayersMoves[playerMask];
            var wallMask = simplePlayersMovesMasks[playerMask];
            var currentWallMask = field.GetWallsForMask(ref wallMask);
            return moves[currentWallMask];
        }
    }
}
