using System.Text;
using System.Text.Json.Serialization;

using XiyouApi.Converter;

namespace XiyouApi
{
    [JsonConverter(typeof(XiyouIDConverter))]
    public unsafe struct XiyouID : IEquatable<XiyouID>
    {
        private fixed byte bytes[32];

        public XiyouID(string str) : this(str.AsSpan()) { }

        public XiyouID(ReadOnlySpan<char> chars)
        {
            fixed (byte* destPtr = bytes)
            {
                var destSpan = new Span<byte>(destPtr, 32);
                var length = Encoding.ASCII.GetBytes(chars, destSpan);
                if (length != 32)
                    throw new ArgumentException("Length have to equals 32 bytes", nameof(chars));
            }
        }

        public XiyouID(XiyouID other)
        {
            byte* sourcePtr = other.bytes;
            fixed (byte* destPtr = bytes)
            {
                Buffer.MemoryCopy(sourcePtr, destPtr, 32, 32);
            }
        }

        public override bool Equals(object? obj) =>
            obj is XiyouID iD && Equals(iD);

        public bool Equals(XiyouID other)
        {
            byte* sourcePtr = other.bytes;
            fixed (byte* destPtr = bytes)
            {
                for (var i = 0; i < 32; i++)
                {
                    if (sourcePtr[i] != destPtr[i])
                        return false;
                }
                return true;
            }
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            fixed (byte* ptr = bytes)
            {
                hashCode.AddBytes(new Span<byte>(ptr, 32));
            }
            return hashCode.ToHashCode();
        }

        public override string ToString()
        {
            fixed (byte* ptr = bytes)
            {
                return Encoding.ASCII.GetString(ptr, 32);
            }
        }

        public static bool operator ==(XiyouID left, XiyouID right) => left.Equals(right);
        public static bool operator !=(XiyouID left, XiyouID right) => !(left == right);
    }
}