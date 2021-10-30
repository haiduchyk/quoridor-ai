namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;

    public class SimpleMoveCalculator
    {
        private const int SimpleMoveBitsAmount = 3;

        // <playerPosition <wallMask, possibleMoves>>
        private Dictionary<(FieldMask playerPosition, FieldMask wallMask), FieldMask[]> simplePlayersMoves = new();

        //  <playerPosition, wallMaskForThisPlayerPosition>
        private Dictionary<FieldMask, FieldMask> simplePlayersMovesMasks = new();

        public SimpleMoveCalculator()
        {
            CreateSimplePlayerMovesMasks();
            CreateSimplePlayerMoves();
        }

        public FieldMask[] GetAvailableMoves(Field field, in FieldMask playerMask)
        {
            // Actual code
            // var wallMask = simplePlayersMovesMasks[playerMask];
            // var currentWallMask = field.GetWallsForMask(in wallMask);
            // var moves = simplePlayersMoves[(playerMask, currentWallMask)];
            // return moves;
            return simplePlayersMoves[(playerMask, field.GetWallsForMask(simplePlayersMovesMasks[playerMask]))];
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
                    var playerMask = new FieldMask();
                    playerMask.SetBit(y, x, true);

                    var uniqueVariant = CalculateUniqueVariantFor(y, x);
                    foreach (var variant in uniqueVariant)
                    {
                        simplePlayersMoves[(playerMask, variant.wallMask)] = variant.movePositions;
                    }
                }
            }
        }

        private List<(FieldMask wallMask, FieldMask[] movePositions)> CalculateUniqueVariantFor(int y, int x)
        {
            var result = new List<(FieldMask, FieldMask[])>();

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

                result.Add((fieldMask, playerMoveMasks.ToArray()));
            }

            return result;
        }
    }
}