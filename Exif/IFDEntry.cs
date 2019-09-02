using System;

namespace WebPFileType.Exif
{
    internal readonly struct IFDEntry : IEquatable<IFDEntry>
    {
        public const int SizeOf = 12;

        public IFDEntry(EndianBinaryReader reader)
        {
            Tag = reader.ReadUInt16();
            Type = (TagDataType)reader.ReadUInt16();
            Count = reader.ReadUInt32();
            Offset = reader.ReadUInt32();
        }

        public IFDEntry(ushort tag, TagDataType type, uint count, uint offset)
        {
            Tag = tag;
            Type = type;
            Count = count;
            Offset = offset;
        }

        public ushort Tag { get; }

        public TagDataType Type { get; }

        public uint Count { get; }

        public uint Offset { get; }

        public override bool Equals(object obj)
        {
            if (obj is IFDEntry entry)
            {
                return Equals(entry);
            }

            return false;
        }

        public bool Equals(IFDEntry other)
        {
            return Tag == other.Tag &&
                   Type == other.Type &&
                   Count == other.Count &&
                   Offset == other.Offset;
        }

        public override int GetHashCode()
        {
            int hashCode = 1198491158;

            unchecked
            {
                hashCode = (hashCode * -1521134295) + Tag.GetHashCode();
                hashCode = (hashCode * -1521134295) + Count.GetHashCode();
                hashCode = (hashCode * -1521134295) + Type.GetHashCode();
                hashCode = (hashCode * -1521134295) + Offset.GetHashCode();
            }

            return hashCode;
        }

        public void Write(System.IO.BinaryWriter writer)
        {
            writer.Write(Tag);
            writer.Write((ushort)Type);
            writer.Write(Count);
            writer.Write(Offset);
        }

        public static bool operator ==(IFDEntry left, IFDEntry right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(IFDEntry left, IFDEntry right)
        {
            return !(left == right);
        }
    }
}
