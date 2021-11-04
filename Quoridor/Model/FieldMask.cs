namespace Quoridor.Model
{
    using System;

    public unsafe struct FieldMask : IEquatable<FieldMask>
    {
        public const int BitsBlockSize = 64;
        public const int BitBlocksAmount = 5;
        public const int BitboardSize = 17;
        public const int BitboardCenter = BitboardSize / 2 + 1;
        public const int PlayerFieldSize = BitboardSize / 2 + 1;
        public const int PlayerFieldArea = PlayerFieldSize * PlayerFieldSize;

        public const int TotalBitsAmount = BitsBlockSize * BitBlocksAmount; // 320, this is with redundant
        public const int UsedBitsAmount = BitboardSize * BitboardSize; // 289, this is without redundant
        public const int ExtraBits = TotalBitsAmount - UsedBitsAmount; // 31 redundant bits

        public fixed long blocks[5];

        public FieldMask(long[] blocks)
        {
            this.blocks[0] = blocks[0];
            this.blocks[1] = blocks[1];
            this.blocks[2] = blocks[2];
            this.blocks[3] = blocks[3];
            this.blocks[4] = blocks[4];
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

        public readonly FieldMask And(in FieldMask mask)
        {
            var result = new FieldMask();

            fixed (long* maskBlocks = mask.blocks)
            {
                fixed (long* oldBlocks = blocks)
                {
                    for (var i = 0; i < BitBlocksAmount; i++)
                    {
                        *(result.blocks + i) = *(oldBlocks + i) & *(maskBlocks + i);
                    }
                }
            }

            return result;
        }

        public readonly FieldMask Or(in FieldMask mask)
        {
            var result = new FieldMask();

            fixed (long* maskBlocks = mask.blocks)
            {
                fixed (long* oldBlocks = blocks)
                {
                    for (var i = 0; i < BitBlocksAmount; i++)
                    {
                        *(result.blocks + i) = *(oldBlocks + i) | *(maskBlocks + i);
                    }
                }
            }

            return result;
        }

        public readonly FieldMask Nor(in FieldMask mask)
        {
            var result = new FieldMask();
            for (var i = 0; i < BitBlocksAmount; i++)
            {
                result[i] = this[i] ^ mask[i];
            }

            return result;
        }

        public readonly FieldMask Not()
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
            return blocks[0] == 0 && blocks[1] == 0 && blocks[2] == 0 && blocks[3] == 0 && blocks[4] == 0;
        }

        public bool IsNotZero()
        {
            return blocks[0] != 0 || blocks[1] != 0 || blocks[2] != 0 || blocks[3] != 0 || blocks[4] != 0;
        }

        private long this[int index]
        {
            readonly get => blocks[index];
            set => blocks[index] = value;
        }

        public static bool IsInRange(int index)
        {
            return index is >= 0 and < UsedBitsAmount;
        }

        public static byte GetPlayerIndex(int y, int x)
        {
            return (byte) (PlayerFieldSize * y / 2 + x / 2);
        }

        public static (int i, int j) GetPlayerPosition(byte position)
        {
            var y = position / PlayerFieldSize;
            var x = position - y * PlayerFieldSize;
            return (y, x);
        }

        public static bool IsInRange(int y, int x)
        {
            return y is >= 0 and < BitboardSize && x is >= 0 and < BitboardSize;
        }

        public bool Equals(FieldMask other)
        {
            return blocks[0] == other.blocks[0] &&
                   blocks[1] == other.blocks[1] &&
                   blocks[2] == other.blocks[2] &&
                   blocks[3] == other.blocks[3] &&
                   blocks[4] == other.blocks[4];
        }

        public override bool Equals(object obj)
        {
            return obj is FieldMask other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(blocks[0], blocks[1], blocks[2], blocks[3], blocks[4]);
        }

        public static bool operator ==(FieldMask left, FieldMask right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FieldMask left, FieldMask right)
        {
            return !left.Equals(right);
        }
    }
}