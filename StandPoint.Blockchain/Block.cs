using System;

namespace StandPoint.Blockchain
{
    public class Block : IComparable<Block>, IEquatable<Block>
    {
        public BlockHeader Header { get; }

        public long Head { set; get; }

        public object Data { set; get; }

        internal Block()
        {
	        Header = new BlockHeader
	        {
		        
	        };
		}

        public Block(byte[] data)
        {
            Header = new BlockHeader(data);
        }

        public bool Equals(Block other)
        {
            return other != null
                && other.Head == this.Head
                && Equals(other.Header, this.Header);
        }

        public int CompareTo(Block other)
        {
            if (this.Head == other.Head) return 0;
            return this.Head > other.Head ? 1 : -1;
        }

        public override string ToString()
        {
            return Header.ToString();
        }
    }
}
