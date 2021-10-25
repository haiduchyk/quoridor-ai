namespace Quoridor.Model
{
    using System;

    // TODO rewrite in fixed buffer array
    public struct FieldMask
    {
        public const int BitsBlockSize = 64;
        public const int BitBlocksAmount = 5;
        public const int BitboardSize = 17;
        public const int BitboardCenter = BitboardSize / 2 + 1;

        public const int TotalBitsAmount = BitsBlockSize * BitBlocksAmount; // 320, this is with redundant
        public const int UsedBitsAmount = BitboardSize * BitboardSize; // 289, this is without redundant
        public const int ExtraBits = TotalBitsAmount - UsedBitsAmount; // 31 redundant bits

        private long block0;
        private long block1;
        private long block2;
        private long block3;
        private long block4;

        public FieldMask(long[] blocks)
        {
            block0 = blocks[0];
            block1 = blocks[1];
            block2 = blocks[2];
            block3 = blocks[3];
            block4 = blocks[4];
        }

        public bool GetBit(int y, int x)
        {
            // TODO: remove check for release
            if (!IsInRange(y, x))
            {
                throw new Exception($"Out of range [y:{y}, x:{x}]");
            }
            var index = x + y * BitboardSize;
            return GetBit(index);
        }

        public bool GetBit(int index)
        {
            var (block, bitIndex) = GetBlockAndBitIndex(index);
            return (block & (1L << bitIndex)) != 0;
        }

        public bool TrySetBit(int y, int x, bool bit)
        {
            var isIsInRange = IsInRange(y, x);
            if (isIsInRange)
            {
                SetBit(y, x, bit);
            }
            return isIsInRange;
        }

        public void SetBit(int y, int x, bool bit)
        {
            var index = x + y * BitboardSize;
            SetBit(index, bit);
        }

        public void SetBit(int index, bool bit)
        {
            var (i, _) = Nest(index);
            var (block, bitIndex) = GetBlockAndBitIndex(index);
            var mask = bit ? 1L : 0L;
            mask <<= bitIndex;
            block |= mask;
            this[i] = block;
        }

        public FieldMask And(in FieldMask mask)
        {
            var result = new FieldMask();
            for (var i = 0; i < BitBlocksAmount; i++)
            {
                result[i] = this[i] & mask[i];
            }
            return result;
        }

        public FieldMask Or(in FieldMask mask)
        {
            var result = new FieldMask();
            for (var i = 0; i < BitBlocksAmount; i++)
            {
                result[i] = this[i] | mask[i];
            }
            return result;
        }

        public FieldMask Nor(in FieldMask mask)
        {
            var result = new FieldMask();
            for (var i = 0; i < BitBlocksAmount; i++)
            {
                result[i] = this[i] ^ mask[i];
            }
            return result;
        }

        public FieldMask Not()
        {
            var result = new FieldMask();
            for (var i = 0; i < BitBlocksAmount; i++)
            {
                result[i] = ~this[i];
            }
            return result;
        }

        public (long block, int bitIndex) GetBlockAndBitIndex(int index)
        {
            var (i, j) = Nest(index);
            var bitIndex = BitsBlockSize - j - 1;
            var block = this[i];
            return (block, bitIndex);
        }

        public (int i, int j) Nest(int index)
        {
            var i = index / BitsBlockSize;
            var j = index % BitsBlockSize;
            return (i, j);
        }

        public bool IsZero()
        {
            return block0 == 0 && block1 == 0 && block2 == 0 && block3 == 0 && block4 == 0;
        }

        public bool IsNotZero()
        {
            return block0 != 0 || block1 != 0 || block2 != 0 || block3 != 0 || block4 != 0;
        }

        private long this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return block0;
                    case 1: return block1;
                    case 2: return block2;
                    case 3: return block3;
                    case 4: return block4;
                    default: throw new Exception("Field index out of range");
                }
            }

            set
            {
                switch (index)
                {
                    case 0:
                        block0 = value;
                        break;
                    case 1:
                        block1 = value;
                        break;
                    case 2:
                        block2 = value;
                        break;
                    case 3:
                        block3 = value;
                        break;
                    case 4:
                        block4 = value;
                        break;
                    default: throw new Exception("Field index out of range");
                }
            }
        }

        public static bool IsInRange(int index)
        {
            return index is >= 0 and < UsedBitsAmount;
        }

        public static bool IsInRange(int y, int x)
        {
            return y is >= 0 and < BitboardSize && x is >= 0 and < BitboardSize;
        }
    }
}
