namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;

    public class SimpleMoveCalculator
    {
        private const int SimpleMoveBitsAmount = 3;

        // <playerPosition <wallMask, possibleMoves>>
        private Dictionary<(byte playerPosition, FieldMask wallMask), byte[]> simplePlayersMoves = new();

        //  <playerPosition, wallMaskForThisPlayerPosition>
        private Dictionary<byte, FieldMask> simplePlayersMovesMasks = new();

        public SimpleMoveCalculator()
        {
            CreateSimplePlayerMovesMasks();
            CreateSimplePlayerMoves();
        }

        public byte[] GetAvailableMoves(Field field, in byte playerIndex)
        {
            // Actual code
            // var wallMask = simplePlayersMovesMasks[playerMask];
            // var currentWallMask = field.GetWallsForMask(in wallMask);
            // var moves = simplePlayersMoves[(playerMask, currentWallMask)];
            // return moves;
            return simplePlayersMoves[(playerIndex, field.GetWallsForMask(simplePlayersMovesMasks[playerIndex]))];
        }

        private void CreateSimplePlayerMovesMasks()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y += 2)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x += 2)
                {
                    var playerIndex = FieldMask.GetPlayerIndex(y, x);
                    simplePlayersMovesMasks[playerIndex] = CreateSimplePlayerMovesMaskFor(y, x);
                }
            }
        }

        private FieldMask CreateSimplePlayerMovesMaskFor(int y, int x)
        {
            var fieldMask = new FieldMask();
            foreach (var (yDelta, xDelta) in Constants.Directions)
            {
                var curY = y + yDelta;
                var curX = x + xDelta;
                fieldMask.TrySetBit(curY, curX, true);
            }

            return fieldMask;
        }

        private void CreateSimplePlayerMoves()
        {
            for (var y = 0; y < FieldMask.BitboardSize; y += 2)
            {
                for (var x = 0; x < FieldMask.BitboardSize; x += 2)
                {
                    var playerIndex = FieldMask.GetPlayerIndex(y, x);
                    var uniqueVariant = CalculateUniqueVariantFor(y, x);
                    foreach (var (wallMask, movePositions) in uniqueVariant)
                    {
                        simplePlayersMoves[(playerIndex, wallMask)] = movePositions;
                    }
                }
            }
        }

        private List<(FieldMask wallMask, byte[] movePositions)> CalculateUniqueVariantFor(int y, int x)
        {
            var result = new List<(FieldMask, byte[])>();

            var uniqueVariants = Math.Pow(4, 2);

            for (var unique = 0; unique < uniqueVariants; unique++)
            {
                var playerMoveMasks = new List<byte>();
                var fieldMask = new FieldMask();

                var uniquePosition = 0;
                for (var yDelta = -SimpleMoveBitsAmount / 2; yDelta < SimpleMoveBitsAmount / 2 + 1; yDelta++)
                {
                    for (var xDelta = -SimpleMoveBitsAmount / 2; xDelta < SimpleMoveBitsAmount / 2 + 1; xDelta++)
                    {
                        var curY = y + yDelta;
                        var curX = x + xDelta;

                        // if y odd and x even || y even and x odd
                        if (Math.Abs(curY) % 2 != Math.Abs(curX) % 2)
                        {
                            UpdateFieldMask();
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
                                        var playerIndex = FieldMask.GetPlayerIndex(newPlayerY, newPlayerX);
                                        playerMoveMasks.Add(playerIndex);
                                    }
                                }
                            }

                            uniquePosition++;
                        }
                    }
                }

                result.Add((fieldMask, playerMoveMasks.ToArray()));
            }

            return result;
        }
    }
}