namespace Quoridor.Model.Strategies
{
    using System;
    using System.Collections.Generic;

    public class SimpleMoveCalculator
    {
        private const int SimpleMoveBitsAmount = 3;

        // <playerPosition <wallMask, possibleMoves>>
        private Dictionary<FieldMask, Dictionary<FieldMask, FieldMask[]>> simplePlayersMoves = new();

        //  <playerPosition, wallMaskForThisPlayerPosition>
        private Dictionary<FieldMask, FieldMask> simplePlayersMovesMasks = new();

        public SimpleMoveCalculator()
        {
            CreateSimplePlayerMovesMasks();
            CreateSimplePlayerMoves();
        }

        public FieldMask[] GetAvailableMoves(Field field, in FieldMask playerMask)
        {
            // Actual code but we optimize these shit
            // var moves = simplePlayersMoves[playerMask];
            // var wallMask = simplePlayersMovesMasks[playerMask];
            // var currentWallMask = field.GetWallsForMask(in wallMask);
            // return moves[currentWallMask];
            return simplePlayersMoves[playerMask][field.GetWallsForMask(simplePlayersMovesMasks[playerMask])];
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

                result[fieldMask] = playerMoveMasks.ToArray();
            }

            return result;
        }
    }
}
