namespace Quoridor.Logic
{
    using System;

    // TODO rewrite in fixed buffer array
    public struct FieldMask
    {
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

        public long this[int index]
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
    }
}